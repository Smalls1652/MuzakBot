namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music artwork.
/// </summary>
public interface IArtwork
{
    string? BackgroundColor { get; set; }
    int Height { get; set; }
    int Width { get; set; }
    string? TextColor1 { get; set; }
    string? TextColor2 { get; set; }
    string? TextColor3 { get; set; }
    string? TextColor4 { get; set; }
    string Url { get; set; }
}