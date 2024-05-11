using System.Diagnostics;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Responses;
using MuzakBot.App.Services;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Slash command for getting the share links for a song/album from a streaming service URL.
    /// </summary>
    /// <param name="url">The URL to a song/album on a streaming service.</param>
    /// <exception cref="Exception"></exception>
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [SlashCommand(
        name: "url",
        description: "Get share links from a streaming service URL."
    )]
    private async Task ShareMusicCommandAsync(
        [Summary(
            name: "url",
            description: "The URL, from a streaming service, of the song or album you want to share."
        )]
        string url
    )
    {
        using var activity = _activitySource.StartShareMusicCommandAsyncActivity(url, Context);

        try
        {
            await DeferAsync();

            MusicEntityItem musicEntityItem;
            try
            {
                musicEntityItem = await GetMusicEntityItemAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No share links found for '{url}'.", url);
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
                _logger.LogError(ex, "Could not get all of the necessary data for '{url}'.", url);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("I was unable to get the necessary information from Odesli. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
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

            await albumArtStream.DisposeAsync();
        }
        finally
        {
            _commandMetrics.IncrementShareMusicCounter();
        }
    }
}
