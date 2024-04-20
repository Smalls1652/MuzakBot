using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
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
        description: "Find music from an artist"
    )]
    private async Task HandleFindSongAsync(
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
        using var activity = _activitySource.StartHandleFindSongAsyncActivity(artistId, songId, Context);

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

            MusicEntityItem? musicEntityItem = null;
            try
            {
                if (songItem.Attributes!.Url is null)
                {
                    throw new Exception("No song item or track view url found.");
                }

                musicEntityItem = await _odesliService.GetShareLinksAsync(songItem.Attributes!.Url);

                if (musicEntityItem is null)
                {
                    throw new Exception("No share links found.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "No share links found for '{url}'.", songItem.Attributes!.Url);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No share links were found. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            PlatformEntityLink? platformEntityLink;
            try
            {
                platformEntityLink = musicEntityItem.LinksByPlatform!["itunes"];
            }
            catch
            {
                /*
                    Temporary fix:
                    
                    Amazon is being excluded from the fallback for the time being,
                    because the "apiProvider" value doesn't cleanly match it's platform
                    entity link key.
                */
                var streamingEntityWithThumbnailUrl = musicEntityItem.EntitiesByUniqueId!.FirstOrDefault(entity => entity.Value.ThumbnailUrl is not null && entity.Value.ApiProvider != "amazon").Value.ApiProvider;

                if (!string.IsNullOrEmpty(streamingEntityWithThumbnailUrl))
                {
                    platformEntityLink = musicEntityItem.LinksByPlatform![streamingEntityWithThumbnailUrl];
                }
                else
                {
                    _logger.LogError("Could get all of the necessary data for '{url}'.", songItem.Attributes.Url);
                    await FollowupAsync(
                        embed: GenerateErrorEmbed("I was unable to get the necessary information from Odesli. 😥").Build(),
                        components: GenerateRemoveComponent().Build()
                    );

                    activity?.SetStatus(ActivityStatusCode.Error);

                    return;
                }
            }

            StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];
            using var albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);

            var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

            string uniqueFileName = Guid.NewGuid().ToString();

            var messageEmbed = new EmbedBuilder()
                .WithTitle(streamingEntityItem.Title)
                .WithDescription($"by {streamingEntityItem.ArtistName}")
                .WithColor(Color.DarkBlue)
                .WithImageUrl($"attachment://{uniqueFileName}.jpg")
                .WithFooter("(Powered by Songlink/Odesli)");

            await FollowupWithFileAsync(
                embed: messageEmbed.Build(),
                fileStream: albumArtStream,
                fileName: $"{uniqueFileName}.jpg",
                components: linksComponentBuilder.Build()
            );
        }
        finally
        {
            _commandMetrics.IncrementFindSongCounter();
        }
    }
}