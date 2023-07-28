using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.Itunes;

public class ArtistItem : IArtistItem
{
    [JsonPropertyName("wrapperType")]
    public string WrapperType { get; set; } = null!;

    [JsonPropertyName("artistType")]
    public string ArtistType { get; set; } = null!;

    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    [JsonPropertyName("artistLinkUrl")]
    public string ArtistLinkUrl { get; set; } = null!;

    [JsonPropertyName("artistId")]
    public long ArtistId { get; set; }

    [JsonPropertyName("amgArtistId")]
    public long? AmgArtistId { get; set; }

    [JsonPropertyName("primaryGenreName")]
    public string PrimaryGenreName { get; set; } = null!;

    [JsonPropertyName("primaryGenreId")]
    public long PrimaryGenreId { get; set; }
}