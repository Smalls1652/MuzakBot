using Discord;
using MuzakBot.App.Models.Odesli;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
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
            .WithButton(youtubeButton)
            .WithButton(appleMusicButton)
            .WithButton(spotifyButton)
            .WithButton(moreLinksButton)
            .WithButton(refreshButton);

        return linksComponentBuilder;
    }
}