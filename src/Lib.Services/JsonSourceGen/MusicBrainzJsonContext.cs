using MuzakBot.Lib.Models.MusicBrainz;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Source generation context for the MusicBrainz models.
/// </summary>
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
[JsonSerializable(typeof(MusicBrainzReleaseSearchResult))]
internal partial class MusicBrainzJsonContext : JsonSerializerContext
{
}
