using MuzakBot.App.Models.Genius;

namespace MuzakBot.App;

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