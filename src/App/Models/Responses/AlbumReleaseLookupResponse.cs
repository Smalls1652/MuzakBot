using System.Text;

using Discord;

using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response for the album release lookup command.
/// </summary>
public sealed class AlbumReleaseLookupResponse : IBotResponse, IDisposable
{
    private bool _isDisposed;

    /// <summary>
    /// Initialize a new instance of the <see cref="AlbumReleaseLookupResponse"/> class.
    /// </summary>
    /// <param name="album">The album to show the release date for.</param>
    /// <param name="musicEntityItem">The music entity item from Odesli.</param>
    /// <param name="isDm">Whether the response is for a DM.</param>
    public AlbumReleaseLookupResponse(Album album, MusicEntityItem musicEntityItem, bool isDm)
    {
        Album = album;
        MusicEntityItem = musicEntityItem;
        IsDm = isDm;
        AlbumArtFileName = $"{Guid.NewGuid():N}.jpg";
        AlbumArtworkStream = Album.Attributes!.Artwork.GetAlbumArtworkStreamAsync(512, 512).GetAwaiter().GetResult();
    }

    /// <summary>
    /// The album to show the release date for.
    /// </summary>
    public Album Album { get; }

    /// <summary>
    /// The music entity item from Odesli.
    /// </summary>
    public MusicEntityItem MusicEntityItem { get; }

    /// <summary>
    /// Whether the response is for a DM.
    /// </summary>
    public bool IsDm { get; }

    /// <summary>
    /// The name being used for the album artwork file.
    /// </summary>
    public string AlbumArtFileName { get; }

    /// <summary>
    /// The stream of the album artwork.
    /// </summary>
    public Stream AlbumArtworkStream { get; }

    /// <inheritdoc/>
    public ComponentBuilder GenerateComponent()
    {
        ComponentBuilder componentBuilder = new ComponentBuilder();

        if (!IsDm)
        {
            componentBuilder
                .WithButton(
                    label: "Remind me",
                    style: ButtonStyle.Primary,
                    customId: $"albumrelease-remindme-{Album.Id}",
                    emote: new Emoji("🔔")
                );
        }

        componentBuilder
            .WithButton(
                label: "Links",
                style: ButtonStyle.Link,
                url: MusicEntityItem.PageUrl!.ToString()
            );

        return componentBuilder;
    }

    /// <inheritdoc/>
    public EmbedBuilder GenerateEmbed()
    {
        DateOnly albumReleaseDate = Album.Attributes!.ReleaseDate;

        DateTimeOffset albumReleaseDateTime = GetEasternDateTimeOffset(albumReleaseDate);

        string description = $@"
by {Album.Attributes!.ArtistName}
### Release Date
<t:{albumReleaseDateTime.ToUnixTimeSeconds()}:f> (<t:{albumReleaseDateTime.ToUnixTimeSeconds()}:R>)

> ⚠️ **Note:**
> The time of release varies depending on your timezone/region.        
";

        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle(Album.Attributes!.Name)
            .WithDescription(description)
            .WithColor(Color.Green)
            .WithImageUrl($"attachment://{AlbumArtFileName}");

        return embedBuilder;
    }

    /// <inheritdoc/>
    public string GenerateText()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the Eastern timezone date time offset for a given date.
    /// </summary>
    /// <param name="date">The input date.</param>
    /// <returns>The Eastern timezone date time offset.</returns>
    private DateTimeOffset GetEasternDateTimeOffset(DateOnly date)
    {
        TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        return new(
            year: date.Year,
            month: date.Month,
            day: date.Day,
            hour: 0,
            minute: 0,
            second: 0,
            offset: easternTimeZone.IsDaylightSavingTime(new(date.Year, date.Month, date.Day, 12, 0, 0))
                ? easternTimeZone.BaseUtcOffset.Add(new(1, 0, 0))
                : easternTimeZone.BaseUtcOffset
        );
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        AlbumArtworkStream.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
