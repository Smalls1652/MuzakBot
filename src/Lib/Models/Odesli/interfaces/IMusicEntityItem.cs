namespace MuzakBot.Lib.Models.Odesli;

/// <summary>
/// Interface that defines properties for a requested entity from Odesli.
/// </summary>
public interface IMusicEntityItem
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    string? EntityUniqueId { get; set; }

    /// <summary>
    /// The country the user requested from the API.
    /// </summary>
    string? UserCountry { get; set; }

    /// <summary>
    /// The URL for the entity's corresponding Odesli page (song.link, album.link).
    /// </summary>
    Uri? PageUrl { get; set; }

    /// <summary>
    /// Links for each music streaming platform the entity is available on.
    /// </summary>
    Dictionary<string, PlatformEntityLink>? LinksByPlatform { get; set; }

    /// <summary>
    /// Data returned from each music streaming platform the entity is available on.
    /// </summary>
    Dictionary<string, StreamingEntityItem>? EntitiesByUniqueId { get; set; }
}
