using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.Lib.Services;

/// <summary>
/// JSON source generation context for the Apple Music API classes.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(Artist))]
[JsonSerializable(typeof(Artist[]))]
[JsonSerializable(typeof(ArtistAttributes))]
[JsonSerializable(typeof(Song))]
[JsonSerializable(typeof(Song[]))]
[JsonSerializable(typeof(SongAttributes))]
[JsonSerializable(typeof(Album))]
[JsonSerializable(typeof(Album[]))]
[JsonSerializable(typeof(AlbumAttributes))]
[JsonSerializable(typeof(AppleMusicResponse<Artist>))]
[JsonSerializable(typeof(AppleMusicResponse<Song>))]
[JsonSerializable(typeof(AppleMusicResponse<Album>))]
[JsonSerializable(typeof(SearchResponse))]
[JsonSerializable(typeof(SearchResponseResults))]
[JsonSerializable(typeof(SearchResponseResultsData<Artist>))]
[JsonSerializable(typeof(SearchResponseResultsData<Song>))]
[JsonSerializable(typeof(SearchResponseResultsData<Album>))]
internal partial class AppleMusicApiJsonContext : JsonSerializerContext
{
}