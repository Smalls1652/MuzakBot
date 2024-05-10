using MuzakBot.Lib.Models.CosmosDb;
using MuzakBot.Lib.Models.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Source generation context for the database models.
/// </summary>
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
[JsonSerializable(typeof(LyricsAnalyzerPromptStyle))]
[JsonSerializable(typeof(LyricsAnalyzerPromptStyle[]))]
[JsonSerializable(typeof(CosmosDbResponse<SongLyricsItem>))]
[JsonSerializable(typeof(CosmosDbResponse<LyricsAnalyzerConfig>))]
[JsonSerializable(typeof(CosmosDbResponse<LyricsAnalyzerUserRateLimit>))]
[JsonSerializable(typeof(CosmosDbResponse<LyricsAnalyzerPromptStyle>))]
internal partial class DatabaseJsonContext : JsonSerializerContext
{ }