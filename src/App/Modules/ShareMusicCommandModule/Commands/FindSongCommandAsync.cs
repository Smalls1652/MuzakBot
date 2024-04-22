using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
using MuzakBot.App.Models.Responses;
using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Itunes;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [SlashCommand(
        name: "findsong",
        description: "Get share links by searching for a song by an artist."
    )]
    private async Task FindSongCommandAsync(
        [Summary("artistName", "The name of an artist"),
         Autocomplete(typeof(AppleMusicArtistAutoCompleteHandler))
        ]
        string artistId,
        [Summary(name: "songName", description: "The name of a song"),
         Autocomplete(typeof(AppleMusicArtistSongAutocompleteHandler))
        ]
        string songId
    )
    {
        using var activity = _activitySource.StartFindSongCommandAsyncActivity(artistId, songId, Context);

        try
        {
            await DeferAsync();

            Artist? artistItem = null;
            try
            {
                artistItem = await _appleMusicApiService.GetArtistFromCatalogAsync(artistId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up artist '{artistId}'.", artistId);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while looking up the artist. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            Song? songItem = null;

            try
            {
                songItem = await _appleMusicApiService.GetSongFromCatalogAsync(songId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up song '{songId}'.", songId);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while looking up the song. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            if (artistItem is null || songItem is null)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No results found for that artist and song. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            MusicEntityItem musicEntityItem;
            try
            {
                musicEntityItem = await GetMusicEntityItemAsync(songItem.Attributes!.Url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No share links found for '{url}'.", songItem.Attributes!.Url);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No share links were found. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            PlatformEntityLink platformEntityLink;
            try
            {
                platformEntityLink = GetPlatformEntityLink(musicEntityItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get all of the necessary data for '{url}'.", songItem.Attributes.Url);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("I was unable to get the necessary information from Odesli. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];
            using var albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);

            ShareMusicResponse shareMusicResponse = new(
                musicEntity: musicEntityItem,
                streamingEntity: streamingEntityItem
            );

            await FollowupWithFileAsync(
                embed: shareMusicResponse.GenerateEmbed().Build(),
                fileStream: albumArtStream,
                fileName: $"{shareMusicResponse.Id}.jpg",
                components: shareMusicResponse.GenerateComponent().Build()
            );
        }
        finally
        {
            _commandMetrics.IncrementFindSongCounter();
        }
    }
}