using System.Text.Json.Serialization;

using MuzakBot.Lib.Models.QueueMessages;

namespace MuzakBot.GeniusService;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(SongLyricsRequestMessage))]
[JsonSerializable(typeof(SongLyricsRequestMessage[]))]
internal partial class QueueMessageJsonContext : JsonSerializerContext
{}