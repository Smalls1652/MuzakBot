namespace MuzakBot.Lib.Models.QueueMessages;

/// <summary>
/// Represents a queue message for requesting song lyrics.
/// </summary>
public sealed class SongLyricsRequestMessage
{
    /*
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    */

    /// <summary>
    /// The URL of the song lyrics on Genius.
    /// </summary>
    [JsonPropertyName("geniusUrl")]
    public string GeniusUrl { get; set; } = null!;
}