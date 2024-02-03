namespace MuzakBot.Lib.Models.Odesli;

/// <summary>
/// Interface that defines properties for a music streaming platform's data on a specific entity.
/// </summary>
public interface IStreamingEntityItem
{
    /// <summary>
    /// The unique identifier on the music streaming platform.
    /// </summary>
    string? Id { get; set; }

    /// <summary>
    /// The name of the song/album on the music streaming platform.
    /// </summary>
    string? Title { get; set; }

    /// <summary>
    /// The name of the artist on the music streaming platform.
    /// </summary>
    string? ArtistName { get; set; }

    /// <summary>
    /// The URL of the thumbnail used on the music streaming platform.
    /// </summary>
    Uri? ThumbnailUrl { get; set; }

    /// <summary>
    /// The width of the thumbnail used on the music streaming platform.
    /// </summary>
    int? ThumbnailWidth { get; set; }

    /// <summary>
    /// The height of the thumbnail used on the music streaming platform.
    /// </summary>
    int? ThumbnailHeight { get; set; }

    /// <summary>
    /// The API provider that returned the match.
    /// </summary>
    string? ApiProvider { get; set; }

    /// <summary>
    /// The music streaming platform(s) the entity is found on.
    /// </summary>
    string[]? Platform { get; set; }
}