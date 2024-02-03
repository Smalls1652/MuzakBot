using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.Odesli;

/// <summary>
/// Holds data for a music streaming platform's data on a specific entity.
/// </summary>
public class StreamingEntityItem : IStreamingEntityItem
{
    /// <inheritdoc cref="IStreamingEntityItem.Id" />
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.Title" />
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.ArtistName" />
    [JsonPropertyName("artistName")]
    public string? ArtistName { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.ThumbnailUrl" />
    [JsonPropertyName("thumbnailUrl")]
    public Uri? ThumbnailUrl { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.ThumbnailWidth" />
    [JsonPropertyName("thumbnailWidth")]
    public int? ThumbnailWidth { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.ThumbnailHeight" />
    [JsonPropertyName("thumbnailHeight")]
    public int? ThumbnailHeight { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.ApiProvider" />
    [JsonPropertyName("apiProvider")]
    public string? ApiProvider { get; set; }

    /// <inheritdoc cref="IStreamingEntityItem.Platform" />
    [JsonPropertyName("platform")]
    public string[]? Platform { get; set; }
}