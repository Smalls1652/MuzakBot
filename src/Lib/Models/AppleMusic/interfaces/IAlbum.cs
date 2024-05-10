namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Interface for an Apple Music album.
/// </summary>
public interface IAlbum
{
    string Id { get; set; }
    string Type { get; set; }
    string Href { get; set; }
    AlbumAttributes? Attributes { get; set; }
}