namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music album's attributes.
/// </summary>
public interface IAlbumAttributes
{
    string ArtistName { get; set; }
    Artwork Artwork { get; set; }
    string? ContentRating { get; set; }
    string? Copyright { get; set; }
    string[] GenreNames { get; set; }
    bool IsCompilation { get; set; }
    bool IsComplete { get; set; }
    bool IsMasteredForItunes { get; set; }
    bool IsSingle { get; set; }
    string Name { get; set; }
    string? RecordLabel { get; set; }
    DateOnly ReleaseDate { get; set; }
    int TrackCount { get; set; }
    string? Upc { get; set; }
    string Url { get; set; }
}
