using System.Text.Json.Serialization;
using MuzakBot.App.Models.Itunes;

namespace MuzakBot.App;

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
internal partial class ItunesJsonContext : JsonSerializerContext
{
}