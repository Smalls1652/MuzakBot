using Discord;
using Discord.Interactions;
using MuzakBot.App.Extensions;
using MuzakBot.Lib.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Slash command for getting the share links for a song/album from a streaming service URL.
    /// </summary>
    /// <param name="url">The URL to a song/album on a streaming service.</param>
    /// <exception cref="Exception"></exception>
    [CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel, InteractionContextType.BotDm)]
    [SlashCommand(
        name: "sharemusic",
        description: "Get share links to a song or album on various streaming platforms."
    )]
    private async Task HandleMusicShareAsync(
        [Summary(
            name: "url",
            description: "The URL, from a streaming service, of the song or album you want to share."
        )]
        string url
    )
    {
        using var activity = _activitySource.StartHandleMusicShareAsyncActivity(url, Context);

        try
        {
            await DeferAsync();

            MusicEntityItem? musicEntityItem = null;
            try
            {
                musicEntityItem = await _odesliService.GetShareLinksAsync(url, activity?.Id);

                if (musicEntityItem is null)
                {
                    throw new Exception("No share links found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No share links found for '{url}'.", url);

                await FollowupAsync(
                    embed: GenerateErrorEmbed("No share links were found. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                throw;
            }

            PlatformEntityLink? platformEntityLink;
            try
            {
                platformEntityLink = musicEntityItem.LinksByPlatform!["itunes"];
            }
            catch (Exception ex)
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
                    _logger.LogError(ex, "Could get all of the necessary data for '{url}'.", url);

                    await FollowupAsync(
                        embed: GenerateErrorEmbed("I was unable to get the necessary information from Odesli. 😥").Build(),
                        components: GenerateRemoveComponent().Build()
                    );

                    activity?.SetStatus(ActivityStatusCode.Error);

                    throw;
                }
            }

            StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];

            Stream albumArtStream;
            try
            {
                albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);
            }
            catch (Exception)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("I ran into an issue while retrieving the album artwork. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                throw;
            }

            var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

            var messageEmbed = GenerateEmbedBuilder(streamingEntityItem);

            await FollowupWithFileAsync(
                embed: messageEmbed.Build(),
                fileStream: albumArtStream,
                fileName: $"{streamingEntityItem.Id}.jpg",
                components: linksComponentBuilder.Build()
            );

            await albumArtStream.DisposeAsync();
        }
        finally
        {
            _commandMetrics.IncrementShareMusicCounter();
        }
    }
}