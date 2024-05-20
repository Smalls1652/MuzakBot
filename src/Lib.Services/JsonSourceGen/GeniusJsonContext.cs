using MuzakBot.Lib.Models.Genius;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Source generation context for the Genius models.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(GeniusArtist))]
[JsonSerializable(typeof(GeniusSearchResult))]
[JsonSerializable(typeof(GeniusSearchResultItem))]
[JsonSerializable(typeof(GeniusSearchResultItem[]))]
[JsonSerializable(typeof(GeniusSearchResultHitItem))]
[JsonSerializable(typeof(GeniusApiResponse<GeniusSearchResult>))]
[JsonSerializable(typeof(GeniusSongItem))]
[JsonSerializable(typeof(GeniusSongResult))]
[JsonSerializable(typeof(GeniusApiResponse<GeniusSongResult>))]
internal partial class GeniusJsonContext : JsonSerializerContext
{ }
