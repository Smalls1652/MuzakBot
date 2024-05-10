namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music artist.
/// </summary>
public interface IArtistAttributes
{
    Artwork? Artwork { get; set; }
    string[] GenreNames { get; set; }
    string Name { get; set; }
    string Url { get; set; }
}