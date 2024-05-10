namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music song.
/// </summary>
public interface ISong
{
    string Id { get; set; }
    string Type { get; set; }
    string Href { get; set; }
    SongAttributes? Attributes { get; set; }
}