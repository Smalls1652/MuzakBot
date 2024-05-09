using System.Text;

using Discord;

using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response for the album release lookup command.
/// </summary>
public sealed class AlbumReleaseLookupResponse : IResponse
{
    /// <summary>
    /// Initialize a new instance of the <see cref="AlbumReleaseLookupResponse"/> class.
    /// </summary>
    /// <param name="album">The album to show the release date for.</param>
    public AlbumReleaseLookupResponse(Album album)
    {
        Album = album;
    }

    /// <summary>
    /// The album to show the release date for.
    /// </summary>
    public Album Album { get; }

    /// <inheritdoc/>
    public ComponentBuilder GenerateComponent()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public EmbedBuilder GenerateEmbed()
    {
        DateOnly albumReleaseDate = Album.Attributes!.ReleaseDate;

        DateTimeOffset albumReleaseDateTime = GetEasternDateTimeOffset(albumReleaseDate);

        StringBuilder descriptionBuilder = new();

        descriptionBuilder
            .AppendLine($"by {Album.Attributes!.ArtistName}")
            .AppendLine("## Release Date")
            .AppendLine()
            .AppendLine()
            .AppendLine($"<t:{albumReleaseDateTime.ToUnixTimeSeconds()}:f> (<t:{albumReleaseDateTime.ToUnixTimeSeconds()}:R>)");

        string artworkUrl = Album.Attributes!.Artwork.Url
            .Replace(
                oldValue: "{w}",
                newValue: "512"
            )
            .Replace(
                oldValue: "{h}",
                newValue: "512"
            );

        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle(Album.Attributes!.Name)
            .WithDescription(descriptionBuilder.ToString())
            .WithColor(Color.Green)
            .WithImageUrl(artworkUrl);

        return embedBuilder;
    }

    /// <inheritdoc/>
    public string GenerateText()
    {
        throw new NotImplementedException();
    }

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

}
