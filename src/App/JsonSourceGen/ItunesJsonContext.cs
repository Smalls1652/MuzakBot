using System.Text.Json.Serialization;
using MuzakBot.Lib.Models.Itunes;

namespace MuzakBot.App;

/// <summary>
/// Source generation context for the iTunes models.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(ArtistItem))]
[JsonSerializable(typeof(ApiSearchResult<ArtistItem>))]
[JsonSerializable(typeof(SongItem))]
[JsonSerializable(typeof(ApiSearchResult<SongItem>))]
[JsonSerializable(typeof(AlbumItem))]
[JsonSerializable(typeof(ApiSearchResult<AlbumItem>))]
internal partial class ItunesJsonContext : JsonSerializerContext
{
}