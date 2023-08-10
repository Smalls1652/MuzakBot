using System.Text.Json.Serialization;
using MuzakBot.App.Models.MusicBrainz;

namespace MuzakBot.App;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(MusicBrainzArtistItem))]
[JsonSerializable(typeof(MusicBrainzArtistSearchResult))]
[JsonSerializable(typeof(MusicBrainzRecordingItem))]
[JsonSerializable(typeof(MusicBrainzRecordingSearchResult))]
[JsonSerializable(typeof(MusicBrainzReleaseItem))]
internal partial class MusicBrainzJsonContext : JsonSerializerContext
{
}