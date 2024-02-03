using MuzakBot.Lib.Models.Genius;

namespace MuzakBot.App;

/// <summary>
/// Source generation context for the Genius models.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(GeniusSearchResult))]
[JsonSerializable(typeof(GeniusSearchResultItem))]
[JsonSerializable(typeof(GeniusSearchResultItem[]))]
[JsonSerializable(typeof(GeniusSearchResultHitItem))]
[JsonSerializable(typeof(GeniusApiResponse<GeniusSearchResult>))]
internal partial class GeniusJsonContext : JsonSerializerContext
{}