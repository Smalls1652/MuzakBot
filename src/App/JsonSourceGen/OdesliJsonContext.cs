using System.Text.Json.Serialization;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App;

/// <summary>
/// Source generation context for the Odesli models.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(StreamingEntityItem))]
[JsonSerializable(typeof(PlatformEntityLink))]
[JsonSerializable(typeof(MusicEntityItem))]
[JsonSerializable(typeof(MusicEntityItem[]))]
[JsonSerializable(typeof(Dictionary<string, IPlatformEntityLink>))]
[JsonSerializable(typeof(Dictionary<string, IStreamingEntityItem>))]
[JsonSerializable(typeof(Dictionary<string, PlatformEntityLink>))]
[JsonSerializable(typeof(Dictionary<string, StreamingEntityItem>))]
internal partial class OdesliJsonContext : JsonSerializerContext
{
}