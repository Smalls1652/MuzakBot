using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.Odesli;

/// <summary>
/// Holds data for a requested entity from Odesli.
/// </summary>
public class MusicEntityItem : IMusicEntityItem
{
    /// <inheritdoc cref="IMusicEntityItem.EntityUniqueId" />
    [JsonPropertyName("entityUniqueId")]
    public string? EntityUniqueId { get; set; }

    /// <inheritdoc cref="IMusicEntityItem.UserCountry" />
    [JsonPropertyName("userCountry")]
    public string? UserCountry { get; set; }

    /// <inheritdoc cref="IMusicEntityItem.PageUrl" />
    [JsonPropertyName("pageUrl")]
    public Uri? PageUrl { get; set; }

    /// <inheritdoc cref="IMusicEntityItem.LinksByPlatform" />
    [JsonPropertyName("linksByPlatform")]
    public Dictionary<string, PlatformEntityLink>? LinksByPlatform { get; set; }

    /// <inheritdoc cref="IMusicEntityItem.EntitiesByUniqueId" />
    [JsonPropertyName("entitiesByUniqueId")]
    public Dictionary<string, StreamingEntityItem>? EntitiesByUniqueId { get; set; }
}