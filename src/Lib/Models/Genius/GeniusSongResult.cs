namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Represents a song result from the Genius API.
/// </summary>
public class GeniusSongResult
{
    /// <summary>
    /// Data for the song.
    /// </summary>
    [JsonPropertyName("song")]
    public GeniusSongItem Song { get; set; } = null!;
}
