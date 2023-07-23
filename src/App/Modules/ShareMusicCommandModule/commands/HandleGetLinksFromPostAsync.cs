using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    [MessageCommand(
        name: "Get share links from post"
    )]
    private async Task HandleGetLinksFromPostAsync(IMessage message)
    {
        Regex linkRegex = new(@"(?'musicLink'(?>https|http):\/\/.+?\/\S+)");

        await DeferAsync(
            ephemeral: true
        );

        _logger.LogInformation("Message content: {messageContent}", message.Content);

        Match linkMatch = linkRegex.Match(message.Content);

        if (!linkMatch.Success)
        {
            await FollowupAsync(
                text: "Could not find a link in that post.",
                ephemeral: true
            );

            return;
        }

        string url = linkMatch.Groups["musicLink"].Value;

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
        using var albumArtStream = await GetMusicEntityItemAlbumArtAsync(streamingEntityItem);

        var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

        var messageEmbed = new EmbedBuilder()
            .WithTitle(streamingEntityItem.Title)
            .WithDescription($"by {streamingEntityItem.ArtistName}")
            .WithColor(Color.DarkBlue)
            .WithFooter("(Powered by Songlink/Odesli)");

        await FollowupAsync(
            text: $"Found a link for `{url}`.",
            ephemeral: true
        );

        await Context.Channel.SendFileAsync(
            embed: messageEmbed.Build(),
            stream: albumArtStream,
            filename: $"{streamingEntityItem.Title}.jpg",
            components: linksComponentBuilder.Build(),
            messageReference: new(message.Id),
            allowedMentions: AllowedMentions.None
        );
    }
}