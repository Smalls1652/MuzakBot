using System.ComponentModel.DataAnnotations.Schema;

using MuzakBot.Lib.Models.QueueMessages;

namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for a song lyrics request job.
/// </summary>
public class SongLyricsRequestJob : DatabaseItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongLyricsRequestJob"/> class.
    /// </summary>
    [JsonConstructor()]
    public SongLyricsRequestJob()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SongLyricsRequestJob"/> class.
    /// </summary>
    /// <param name="requestMessage">The <see cref="SongLyricsRequestMessage"/>.</param>
    public SongLyricsRequestJob(SongLyricsRequestMessage requestMessage)
    {
        Id = requestMessage.JobId;
        PartitionKey = "song-lyrics-request-job";
        GeniusUrl = requestMessage.GeniusUrl;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SongLyricsRequestJob"/> class.
    /// </summary>
    /// <param name="geniusUrl">The URL to the song lyrics on Genius.</param>
    public SongLyricsRequestJob(string geniusUrl)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "song-lyrics-request-job";
        GeniusUrl = geniusUrl;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// The URL to the song lyrics on Genius.
    /// </summary>
    [Column("geniusUrl")]
    [JsonPropertyName("geniusUrl")]
    public string GeniusUrl { get; set; } = null!;

    /// <summary>
    /// The date and time the song lyrics request job was created.
    /// </summary>
    [Column("createdAt")]
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Whether the standalone service has acknowledged the request.
    /// </summary>
    [Column("standaloneServiceAcknowledged")]
    [JsonPropertyName("standaloneServiceAcknowledged")]
    public bool StandaloneServiceAcknowledged { get; set; }

    /// <summary>
    /// Whether the fallback method is needed.
    /// </summary>
    [Column("fallbackMethodNeeded")]
    [JsonPropertyName("fallbackMethodNeeded")]
    public bool FallbackMethodNeeded { get; set; }

    /// <summary>
    /// Whether the job is completed.
    /// </summary>
    [Column("isCompleted")]
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set; }

    /// <summary>
    /// The ID of the song lyrics item.
    /// </summary>
    [Column("songLyricsItemId")]
    [JsonPropertyName("songLyricsItemId")]
    public string? SongLyricsItemId { get; set; }
}