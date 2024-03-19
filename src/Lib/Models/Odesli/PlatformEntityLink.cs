using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.Odesli;

/// <summary>
/// Holds data for a music streaming platform's links to an entity.
/// </summary>
public class PlatformEntityLink : IPlatformEntityLink
{
    /// <inheritdoc cref="IPlatformEntityLink.EntityUniqueId" />
    [JsonPropertyName("entityUniqueId")]
    public string? EntityUniqueId { get; set; }

    /// <inheritdoc cref="IPlatformEntityLink.Url" />
    [JsonPropertyName("url")]
    public Uri? Url { get; set; }

    /// <inheritdoc cref="IPlatformEntityLink.NativeAppUriMobile" />
    [JsonPropertyName("nativeAppUriMobile")]
    public Uri? NativeAppUriMobile { get; set; }

    /// <inheritdoc cref="IPlatformEntityLink.NativeAppUriDesktop" />
    [JsonPropertyName("nativeAppUriDesktop")]
    public Uri? NativeAppUriDesktop { get; set; }
}