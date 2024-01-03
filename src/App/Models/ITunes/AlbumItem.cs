using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.Itunes;

/// <summary>
/// Represents an album item retrieved from iTunes.
/// </summary>
public class AlbumItem : IAlbumItem
{
    /// <summary>
    /// Gets or sets the wrapper type of the album item.
    /// </summary>
    [JsonPropertyName("wrapperType")]
    public string WrapperType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection type of the album item.
    /// </summary>
    [JsonPropertyName("collectionType")]
    public string CollectionType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the artist ID of the album item.
    /// </summary>
    [JsonPropertyName("artistId")]
    public long ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the collection ID of the album item.
    /// </summary>
    [JsonPropertyName("collectionId")]
    public long CollectionId { get; set; }

    /// <summary>
    /// Gets or sets the artist name of the album item.
    /// </summary>
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection name of the album item.
    /// </summary>
    [JsonPropertyName("collectionName")]
    public string CollectionName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the censored collection name of the album item.
    /// </summary>
    [JsonPropertyName("collectionCensoredName")]
    public string? CollectionCensoredName { get; set; }

    /// <summary>
    /// Gets or sets the collection view URL of the album item.
    /// </summary>
    [JsonPropertyName("collectionViewUrl")]
    public string CollectionViewUrl { get; set; } = null!;
}