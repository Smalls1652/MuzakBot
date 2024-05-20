namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Represents a song item from the Genius API.
/// </summary>
public class GeniusSongItem
{
    /// <summary>
    /// The ID of the song.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// The title of the song.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    /// <summary>
    /// The URL of the song.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    /// <summary>
    /// The ID of the song on Apple Music.
    /// </summary>
    [JsonPropertyName("apple_music_id")]
    public string? AppleMusicId { get; set; }
}
