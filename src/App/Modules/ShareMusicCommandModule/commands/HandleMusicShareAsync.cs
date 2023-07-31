using Discord;
using Discord.Interactions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Slash command for getting the share links for a song/album from a streaming service URL.
    /// </summary>
    /// <param name="url">The URL to a song/album on a streaming service.</param>
    /// <exception cref="Exception"></exception>
    [EnabledInDm(true)]
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
        await DeferAsync();

        MusicEntityItem? musicEntityItem = null;
        try
        {
            musicEntityItem = await _odesliService.GetShareLinksAsync(url);

            if (musicEntityItem is null)
            {
                throw new Exception("No share links found.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "No share links found for '{url}'.", url);
            await FollowupAsync(
                text: "No share links were found for that URL. 😥",
                components: GenerateRemoveComponent().Build()
            );

            return;
        }

        PlatformEntityLink? platformEntityLink;
        try
        {
            platformEntityLink = musicEntityItem.LinksByPlatform!["itunes"];
        }
        catch
        {
            var streamingEntityWithThumbnailUrl = musicEntityItem.EntitiesByUniqueId!.FirstOrDefault(entity => entity.Value.ThumbnailUrl is not null).Value.ApiProvider;

            if (!string.IsNullOrEmpty(streamingEntityWithThumbnailUrl))
            {
                platformEntityLink = musicEntityItem.LinksByPlatform![streamingEntityWithThumbnailUrl];
            }
            else
            {
                _logger.LogError("Could get all of the necessary data for '{url}'.", url);
                await FollowupAsync(
                    text: "I was unable to get the necessary information from Odesli. 😥",
                    components: GenerateRemoveComponent().Build()
                );

                return;
            }
        }

        StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];
        await using var albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);

        var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

        var messageEmbed = GenerateEmbedBuilder(streamingEntityItem);

        await FollowupWithFileAsync(
            embed: messageEmbed.Build(),
            fileStream: albumArtStream,
            fileName: $"{streamingEntityItem.Id}.jpg",
            components: linksComponentBuilder.Build()
        );


    }
}