namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music artist.
/// </summary>
public interface IArtist
{
    string Id { get; set; }
    string Type { get; set; }
    string Href { get; set; }
    ArtistAttributes? Attributes { get; set; }
}