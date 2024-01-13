using MuzakBot.App.Models.CosmosDb;
using MuzakBot.App.Models.Database;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
)]
[JsonSerializable(typeof(DatabaseItem))]
[JsonSerializable(typeof(SongLyricsItem))]
[JsonSerializable(typeof(SongLyricsItem[]))]
[JsonSerializable(typeof(LyricsAnalyzerConfig))]
[JsonSerializable(typeof(LyricsAnalyzerUserRateLimit))]
[JsonSerializable(typeof(LyricsAnalyzerUserRateLimit[]))]
[JsonSerializable(typeof(CosmosDbResponse<SongLyricsItem>))]
[JsonSerializable(typeof(CosmosDbResponse<LyricsAnalyzerConfig>))]
[JsonSerializable(typeof(CosmosDbResponse<LyricsAnalyzerUserRateLimit>))]
internal partial class DatabaseJsonContext : JsonSerializerContext
{}