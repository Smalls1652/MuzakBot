using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.Itunes;

/// <summary>
/// Represents a song item retrieved from iTunes.
/// </summary>
public class SongItem : ISongItem
{
    /// <summary>
    /// Gets or sets the wrapper type of the song item.
    /// </summary>
    [JsonPropertyName("wrapperType")]
    public string? WrapperType { get; set; }

    /// <summary>
    /// Gets or sets the kind of the song item.
    /// </summary>
    [JsonPropertyName("kind")]
    public string? Kind { get; set; }

    /// <summary>
    /// Gets or sets the artist ID of the song item.
    /// </summary>
    [JsonPropertyName("artistId")]
    public long ArtistId { get; set; }

    /// <summary>
    /// Gets or sets the collection ID of the song item.
    /// </summary>
    [JsonPropertyName("collectionId")]
    public long CollectionId { get; set; }

    /// <summary>
    /// Gets or sets the track ID of the song item.
    /// </summary>
    [JsonPropertyName("trackId")]
    public long TrackId { get; set; }

    /// <summary>
    /// Gets or sets the artist name of the song item.
    /// </summary>
    [JsonPropertyName("artistName")]
    public string? ArtistName { get; set; }

    /// <summary>
    /// Gets or sets the collection name of the song item.
    /// </summary>
    [JsonPropertyName("collectionName")]
    public string? CollectionName { get; set; }

    /// <summary>
    /// Gets or sets the track name of the song item.
    /// </summary>
    [JsonPropertyName("trackName")]
    public string? TrackName { get; set; }

    /// <summary>
    /// Gets or sets the censored collection name of the song item.
    /// </summary>
    [JsonPropertyName("collectionCensoredName")]
    public string? CollectionCensoredName { get; set; }

    /// <summary>
    /// Gets or sets the censored track name of the song item.
    /// </summary>
    [JsonPropertyName("trackCensoredName")]
    public string? TrackCensoredName { get; set; }

    /// <summary>
    /// Gets or sets the artist view URL of the song item.
    /// </summary>
    [JsonPropertyName("artistViewUrl")]
    public string? ArtistViewUrl { get; set; }

    /// <summary>
    /// Gets or sets the collection view URL of the song item.
    /// </summary>
    [JsonPropertyName("collectionViewUrl")]
    public string? CollectionViewUrl { get; set; }

    /// <summary>
    /// Gets or sets the track view URL of the song item.
    /// </summary>
    [JsonPropertyName("trackViewUrl")]
    public string? TrackViewUrl { get; set; }

    /// <summary>
    /// Gets or sets the preview URL of the song item.
    /// </summary>
    [JsonPropertyName("previewUrl")]
    public string? PreviewUrl { get; set; }

    /// <summary>
    /// Gets or sets the artwork URL (30x30) of the song item.
    /// </summary>
    [JsonPropertyName("artworkUrl30")]
    public string? ArtworkUrl30 { get; set; }

    /// <summary>
    /// Gets or sets the artwork URL (60x60) of the song item.
    /// </summary>
    [JsonPropertyName("artworkUrl60")]
    public string? ArtworkUrl60 { get; set; }

    /// <summary>
    /// Gets or sets the artwork URL (100x100) of the song item.
    /// </summary>
    [JsonPropertyName("artworkUrl100")]
    public string? ArtworkUrl100 { get; set; }

    /// <summary>
    /// Gets or sets the collection price of the song item.
    /// </summary>
    [JsonPropertyName("collectionPrice")]
    public double CollectionPrice { get; set; }

    /// <summary>
    /// Gets or sets the track price of the song item.
    /// </summary>
    [JsonPropertyName("trackPrice")]
    public double TrackPrice { get; set; }

    /// <summary>
    /// Gets or sets the release date of the song item.
    /// </summary>
    [JsonPropertyName("releaseDate")]
    public DateTimeOffset ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets the collection explicitness of the song item.
    /// </summary>
    [JsonPropertyName("collectionExplicitness")]
    public string? CollectionExplicitness { get; set; }

    /// <summary>
    /// Gets or sets the track explicitness of the song item.
    /// </summary>
    [JsonPropertyName("trackExplicitness")]
    public string? TrackExplicitness { get; set; }

    /// <summary>
    /// Gets or sets the disc count of the song item.
    /// </summary>
    [JsonPropertyName("discCount")]
    public int DiscCount { get; set; }

    /// <summary>
    /// Gets or sets the disc number of the song item.
    /// </summary>
    [JsonPropertyName("discNumber")]
    public int DiscNumber { get; set; }

    /// <summary>
    /// Gets or sets the track count of the song item.
    /// </summary>
    [JsonPropertyName("trackCount")]
    public int TrackCount { get; set; }

    /// <summary>
    /// Gets or sets the track number of the song item.
    /// </summary>
    [JsonPropertyName("trackNumber")]
    public int TrackNumber { get; set; }

    /// <summary>
    /// Gets or sets the track time in milliseconds of the song item.
    /// </summary>
    [JsonPropertyName("trackTimeMillis")]
    public long TrackTimeMillis { get; set; }

    /// <summary>
    /// Gets or sets the country of the song item.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the currency of the song item.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the primary genre name of the song item.
    /// </summary>
    [JsonPropertyName("primaryGenreName")]
    public string? PrimaryGenreName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the song item is streamable.
    /// </summary>
    [JsonPropertyName("isStreamable")]
    public bool IsStreamable { get; set; }
}