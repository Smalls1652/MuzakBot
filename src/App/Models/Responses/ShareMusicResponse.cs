using Discord;

using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response for the share music command.
/// </summary>
public class ShareMusicResponse : IBotResponse
{
    private readonly PlatformEntityLink? _youtubeLink;
    private readonly PlatformEntityLink? _appleMusicLink;
    private readonly PlatformEntityLink? _spotifyLink;
    private readonly PlatformEntityLink? _soundcloudLink;

    /// <summary>
    /// Initialize a new instance of the <see cref="ShareMusicResponse"/> class.
    /// </summary>
    /// <param name="musicEntity">The music entity data.</param>
    public ShareMusicResponse(MusicEntityItem musicEntity)
    {
        Id = Guid.NewGuid().ToString();
        MusicEntity = musicEntity;

        _youtubeLink = GetPlatformEntityLink(musicEntity, "youtube");
        _appleMusicLink = GetPlatformEntityLink(musicEntity, "appleMusic");
        _spotifyLink = GetPlatformEntityLink(musicEntity, "spotify");
        _soundcloudLink = GetPlatformEntityLink(musicEntity, "soundcloud");
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="ShareMusicResponse"/> class.
    /// </summary>
    /// <param name="musicEntity">The music entity data.</param>
    /// <param name="streamingEntity">The streaming entity data.</param>
    public ShareMusicResponse(MusicEntityItem musicEntity, StreamingEntityItem streamingEntity)
    {
        Id = Guid.NewGuid().ToString();
        MusicEntity = musicEntity;
        StreamingEntity = streamingEntity;

        _youtubeLink = GetPlatformEntityLink(musicEntity, "youtube");
        _appleMusicLink = GetPlatformEntityLink(musicEntity, "appleMusic");
        _spotifyLink = GetPlatformEntityLink(musicEntity, "spotify");
        _soundcloudLink = GetPlatformEntityLink(musicEntity, "soundcloud");
    }

    /// <summary>
    /// A unique identifier for the response.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The music entity data.
    /// </summary>
    public MusicEntityItem MusicEntity { get; }

    /// <summary>
    /// The streaming entity data.
    /// </summary>
    public StreamingEntityItem? StreamingEntity { get; }

    /// <inheritdoc />
    public ComponentBuilder GenerateComponent()
    {
        // Create the YouTube button component.
        ButtonBuilder youtubeButton;
        if (_youtubeLink is not null)
        {
            // If the YouTube link is not null, create a button component with the link.
            youtubeButton = new(
                label: "YouTube",
                style: ButtonStyle.Link,
                url: _youtubeLink.Url!.ToString()
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
                customId: $"{MusicEntity.EntityUniqueId}-youtube-disabled"
            );
        }

        // Create the Apple Music button component.
        ButtonBuilder appleMusicButton;
        if (_appleMusicLink is not null)
        {
            // If the Apple Music link is not null, create a button component with the link.
            appleMusicButton = new(
                label: "Apple Music",
                style: ButtonStyle.Link,
                url: _appleMusicLink.Url!.ToString()
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
                customId: $"{MusicEntity.EntityUniqueId}-appleMusic-disabled"
            );
        }

        // Create the Spotify button component.
        ButtonBuilder spotifyButton;
        if (_spotifyLink is not null)
        {
            // If the Spotify link is not null, create a button component with the link.
            spotifyButton = new(
                label: "Spotify",
                style: ButtonStyle.Link,
                url: _spotifyLink.Url!.ToString()
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
                customId: $"{MusicEntity.EntityUniqueId}-spotify-disabled"
            );
        }

        // Create the SoundCloud button component.
        ButtonBuilder soundcloudButton;
        if (_soundcloudLink is not null)
        {
            // If the SoundCloud link is not null, create a button component with the link.
            soundcloudButton = new(
                label: "SoundCloud",
                style: ButtonStyle.Link,
                url: _soundcloudLink.Url!.ToString()
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
                customId: $"{MusicEntity.EntityUniqueId}-soundcloud-disabled"
            );
        }

        // Create the "More links" button component.
        ButtonBuilder moreLinksButton = new(
            label: "More links",
            style: ButtonStyle.Link,
            url: MusicEntity.PageUrl!.ToString()
        );

        // Create the return button component.
        ButtonBuilder refreshButton = new(
            label: "Refresh",
            style: ButtonStyle.Secondary,
            emote: new Emoji("🔄"),
            customId: $"refresh-musiclinks-btn-{MusicEntity.PageUrl}"
        );

        // Create the component builder from the button components.
        ComponentBuilder linksComponentBuilder = new ComponentBuilder()
            .WithButton(youtubeButton, 0)
            .WithButton(appleMusicButton, 0)
            .WithButton(spotifyButton, 1)
            .WithButton(soundcloudButton, 1)
            .WithButton(moreLinksButton, 2)
            .WithButton(refreshButton, 2);

        return linksComponentBuilder;
    }

    /// <inheritdoc />
    public EmbedBuilder GenerateEmbed()
    {
        if (StreamingEntity is null)
        {
            throw new NullReferenceException("No streaming entity data was included.");
        }

        return new EmbedBuilder()
            .WithTitle(StreamingEntity.Title)
            .WithDescription($"by {StreamingEntity.ArtistName}")
            .WithColor(Color.DarkBlue)
            .WithImageUrl($"attachment://{Id}.jpg")
            .WithFooter("(Powered by Songlink/Odesli)");
    }

    /// <inheritdoc />
    public string GenerateText() => throw new NotImplementedException("This method is not implemented for this response type.");

    /// <summary>
    /// Get the platform entity link for a specified platform.
    /// </summary>
    /// <param name="entityItem">The music entity item.</param>
    /// <param name="platform">The platform to get the link for.</param>
    /// <returns>The platform entity link.</returns>
    private static PlatformEntityLink? GetPlatformEntityLink(MusicEntityItem entityItem, string platform)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
            });
        });

        ILogger<ShareMusicResponse> logger = loggerFactory.CreateLogger<ShareMusicResponse>();

        PlatformEntityLink? platformEntityLink = null;
        try
        {
            platformEntityLink = entityItem.LinksByPlatform![platform];
        }
        catch
        {
            logger.LogWarning("No '{platform}' link found for {url}", platform, entityItem.PageUrl);
        }

        return platformEntityLink;
    }
}
