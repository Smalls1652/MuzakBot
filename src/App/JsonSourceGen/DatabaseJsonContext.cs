using MuzakBot.App.Models.CosmosDb;
using MuzakBot.App.Models.Database;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(DatabaseItem))]
[JsonSerializable(typeof(SongLyricsItem))]
[JsonSerializable(typeof(SongLyricsItem[]))]
[JsonSerializable(typeof(CosmosDbResponse<SongLyricsItem>))]
internal partial class DatabaseJsonContext : JsonSerializerContext
{}