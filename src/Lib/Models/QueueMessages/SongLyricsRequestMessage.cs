namespace MuzakBot.Lib.Models.QueueMessages;

/// <summary>
/// Represents a queue message for requesting song lyrics.
/// </summary>
public sealed class SongLyricsRequestMessage
{
    [JsonPropertyName("jobId")]
    public string JobId { get; set; } = null!;

    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    [JsonPropertyName("songTitle")]
    public string SongTitle { get; set; } = null!;

    /// <summary>
    /// The URL of the song lyrics on Genius.
    /// </summary>
    [JsonPropertyName("geniusUrl")]
    public string GeniusUrl { get; set; } = null!;
}
