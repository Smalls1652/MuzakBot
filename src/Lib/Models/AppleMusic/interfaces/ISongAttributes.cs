namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music song's attributes.
/// </summary>
public interface ISongAttributes
{
    string AlbumName { get; set; }
    string ArtistName { get; set; }
    string ArtistUrl { get; set; }
    Artwork Artwork { get; set; }
    string? ComposerName { get; set; }
    string? ContentRating { get; set; }
    int DiscNumber { get; set; }
    int DurationInMillis { get; set; }
    string[] GenreNames { get; set; }
    bool HasLyrics { get; set; }
    string? Isrc { get; set; }
    string Name { get; set; }
    DateOnly ReleaseDate { get; set; }
    int TrackNumber { get; set; }
    string Url { get; set; }
}
