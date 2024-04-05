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
[JsonSerializable(typeof(SearchResponse<Artist>))]
[JsonSerializable(typeof(SearchResponse<Song>))]
[JsonSerializable(typeof(SearchResponse<Album>))]
[JsonSerializable(typeof(SearchResponseResults<Artist>))]
[JsonSerializable(typeof(SearchResponseResults<Song>))]
[JsonSerializable(typeof(SearchResponseResults<Album>))]
internal partial class AppleMusicApiJsonContext : JsonSerializerContext
{
}
