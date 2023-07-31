using Discord;
using MuzakBot.App.Models.Odesli;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Genereates a <see cref="ComponentBuilder"/> for the music share links.
    /// </summary>
    /// <param name="entityItem">The data from the Odesli API for the song/album.</param>
    /// <returns></returns>
    private ComponentBuilder GenerateMusicShareComponent(MusicEntityItem entityItem)
    {
        // Attempt to get the YouTube link for the music item.
        PlatformEntityLink? youtubeLink;
        try
        {
            youtubeLink = entityItem.LinksByPlatform!["youtube"];
        }
        catch
        {
            // If the YouTube link is not found, set it to null.
            _logger.LogWarning("No YouTube link found for {url}", entityItem.PageUrl);
            youtubeLink = null;
        }

        // Attempt to get the Apple Music link for the music item.
        PlatformEntityLink? appleMusicLink;
        try
        {
            appleMusicLink = entityItem.LinksByPlatform!["appleMusic"];
        }
        catch
        {
            // If the Apple Music link is not found, set it to null.
            _logger.LogWarning("No Apple Music link found for {url}", entityItem.PageUrl);
            appleMusicLink = null;
        }

        // Attempt to get the Spotify link for the music item.
        PlatformEntityLink? spotifyLink;
        try
        {
            spotifyLink = entityItem.LinksByPlatform!["spotify"];
        }
        catch
        {
            _logger.LogWarning("No Spotify link found for {url}", entityItem.PageUrl);
            spotifyLink = null;
        }

        // Attempt to get the Spotify link for the music item.
        PlatformEntityLink? soundcloudLink;
        try
        {
            soundcloudLink = entityItem.LinksByPlatform!["soundcloud"];
        }
        catch
        {
            _logger.LogWarning("No SoundCloud link found for {url}", entityItem.PageUrl);
            soundcloudLink = null;
        }

        // Create the YouTube button component.
        ButtonBuilder youtubeButton;
        if (youtubeLink is not null)
        {
            // If the YouTube link is not null, create a button component with the link.
            youtubeButton = new(
                label: "YouTube",
                style: ButtonStyle.Link,
                url: youtubeLink.Url!.ToString()
            );
        }
        else
        {
            // If the YouTube link is null, create a button component with a disabled label.
            youtubeButton = new(
                label: "YouTube",
                style: ButtonStyle.Secondary,
                isDisabled: true,
                emote: new Emoji("🚫"),
                customId: $"{entityItem.EntityUniqueId}-youtube-disabled"
            );
        }

        // Create the Apple Music button component.
        ButtonBuilder appleMusicButton;
        if (appleMusicLink is not null)
        {
            // If the Apple Music link is not null, create a button component with the link.
            appleMusicButton = new(
                label: "Apple Music",
                style: ButtonStyle.Link,
                url: appleMusicLink.Url!.ToString()
            );
        }
        else
        {
            // If the Apple Music link is null, create a button component with a disabled label.
            appleMusicButton = new(
                label: "Apple Music",
                style: ButtonStyle.Secondary,
                isDisabled: true,
                emote: new Emoji("🚫"),
                customId: $"{entityItem.EntityUniqueId}-appleMusic-disabled"
            );
        }

        // Create the Spotify button component.
        ButtonBuilder spotifyButton;
        if (spotifyLink is not null)
        {
            // If the Spotify link is not null, create a button component with the link.
            spotifyButton = new(
                label: "Spotify",
                style: ButtonStyle.Link,
                url: spotifyLink.Url!.ToString()
            );
        }
        else
        {
            // If the Spotify link is null, create a button component with a disabled label.
            spotifyButton = new(
                label: "Spotify",
                style: ButtonStyle.Secondary,
                isDisabled: true,
                emote: new Emoji("🚫"),
                customId: $"{entityItem.EntityUniqueId}-spotify-disabled"
            );
        }

        // Create the SoundCloud button component.
        ButtonBuilder soundcloudButton;
        if (soundcloudLink is not null)
        {
            // If the SoundCloud link is not null, create a button component with the link.
            soundcloudButton = new(
                label: "SoundCloud",
                style: ButtonStyle.Link,
                url: soundcloudLink.Url!.ToString()
            );
        }
        else
        {
            // If the SoundCloud link is null, create a button component with a disabled label.
            soundcloudButton = new(
                label: "SoundCloud",
                style: ButtonStyle.Secondary,
                isDisabled: true,
                emote: new Emoji("🚫"),
                customId: $"{entityItem.EntityUniqueId}-soundcloud-disabled"
            );
        }

        // Create the "More links" button component.
        ButtonBuilder moreLinksButton = new(
            label: "More links",
            style: ButtonStyle.Link,
            url: entityItem.PageUrl!.ToString()
        );

        // Create the return button component.
        ButtonBuilder refreshButton = new(
            label: "Refresh",
            style: ButtonStyle.Secondary,
            emote: new Emoji("🔄"),
            customId: $"refresh-musiclinks-btn-{entityItem.PageUrl}"
        );

        // Create the component builder from the button components.
        ComponentBuilder linksComponentBuilder = new ComponentBuilder()
            .WithButton(youtubeButton, 0)
            .WithButton(appleMusicButton, 0)
            .WithButton(spotifyButton, 0)
            .WithButton(soundcloudButton, 0)
            .WithButton(moreLinksButton, 1)
            .WithButton(refreshButton, 1);

        return linksComponentBuilder;
    }
}