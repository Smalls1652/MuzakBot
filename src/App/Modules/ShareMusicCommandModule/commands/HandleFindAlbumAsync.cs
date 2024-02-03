using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
using MuzakBot.Lib.Models.Itunes;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    [SlashCommand(
        name: "findalbum",
        description: "Find an album from an artist"
    )]
    private async Task HandleFindAlbumAsync(
        [Summary("artistName", "The name of an artist"),
         Autocomplete(typeof(ArtistSearchAutoCompleteHandler))
        ]
        string artistId,
        [Summary(name: "albumName", description: "The name of an album"),
         Autocomplete(typeof(ArtistAlbumSearchAutoCompleteHandler))
        ]
        string albumId
    )
    {
        using var activity = _activitySource.StartHandleFindAlbumAsyncActivity(artistId, albumId, Context);
        
        try
        {
            await DeferAsync(
                ephemeral: false
            );

            MusicBrainzArtistItem? artistItem = null;
            try
            {
                artistItem = await _musicBrainzService.LookupArtistAsync(artistId, activity?.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up artist '{artistId}'.", artistId);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while looking up the artist. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            MusicBrainzReleaseItem? releaseItem = null;

            try
            {
                releaseItem = await _musicBrainzService.LookupReleaseAsync(albumId, activity?.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up album '{songId}'.", albumId);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while looking up the album. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            if (artistItem is null || releaseItem is null)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No results found for that artist and album. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            ApiSearchResult<AlbumItem>? apiSearchResult = await _itunesApiService.GetAlbumsByArtistResultAsync(artistItem!.Name, releaseItem!.Title, activity?.Id);

            if (apiSearchResult is null || apiSearchResult.Results is null || apiSearchResult.Results.Length == 0)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No results were found.").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }


            AlbumItem albumItem = apiSearchResult.Results[0];

            _logger.LogInformation("Selected album '{albumName}' by '{artistName}'.", albumItem.CollectionName, albumItem.ArtistName);

            MusicEntityItem? musicEntityItem = null;
            try
            {
                musicEntityItem = await _odesliService.GetShareLinksAsync(albumItem.CollectionViewUrl);

                if (musicEntityItem is null)
                {
                    throw new Exception("No share links found.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "No share links found for '{url}'.", albumItem.CollectionViewUrl);
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
                    _logger.LogError("Could get all of the necessary data for '{url}'.", albumItem.CollectionViewUrl);
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
            _commandMetrics.IncrementFindAlbumCounter();
        }
    }
}