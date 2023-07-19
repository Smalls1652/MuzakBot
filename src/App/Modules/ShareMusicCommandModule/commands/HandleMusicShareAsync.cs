using Discord;
using Discord.Interactions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
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

        MusicEntityItem? musicEntityItem = await _odesliService.GetShareLinksAsync(url);

        if (musicEntityItem is null)
        {
            _logger.LogWarning("No share links found for '{url}'.", url);
            await FollowupAsync(
                text: "No share links were found for that URL. 😥",
                ephemeral: true
            );

            return;
        }

        PlatformEntityLink? itunesLink;
        try
        {
            itunesLink = musicEntityItem.LinksByPlatform!["itunes"];
        }
        catch
        {
            _logger.LogError("No iTunes link found for '{url}'.", url);
            await FollowupAsync(
                text: "I was unable to get the necessary information from Odesli. 😥",
                ephemeral: true
            );

            return;
        }

        StreamingEntityItem itunesEntityItem = musicEntityItem.EntitiesByUniqueId![itunesLink.EntityUniqueId!];
        using var albumArtStream = await GetMusicEntityItemAlbumArtAsync(itunesEntityItem);

        var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

        var messageEmbed = new EmbedBuilder()
            .WithTitle(itunesEntityItem.Title)
            .WithDescription($"by {itunesEntityItem.ArtistName}")
            .WithColor(Color.DarkBlue)
            .WithFooter("(Powered by Songlink/Odesli)");

        await FollowupWithFileAsync(
            //text: $"Streaming music links for **{itunesEntityItem.Title} by {itunesEntityItem.ArtistName}**",
            embed: messageEmbed.Build(), 
            fileStream: albumArtStream,
            fileName: $"{itunesEntityItem.Title}.jpg",
            components: linksComponentBuilder.Build()
        );

    }
}