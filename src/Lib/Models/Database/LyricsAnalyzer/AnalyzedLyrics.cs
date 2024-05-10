using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for analyzed lyrics.
/// </summary>
[Table("analyzed_lyrics")]
public class AnalyzedLyrics : DatabaseItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzedLyrics"/> class.
    /// </summary>
    [JsonConstructor]
    public AnalyzedLyrics()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyzedLyrics"/> class.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="promptStyleUsed">The style of the prompt used for the completion.</param>
    /// <param name="songLyricsId">The ID of the song lyrics.</param>
    /// <param name="analysis">The analysis of the lyrics.</param>
    public AnalyzedLyrics(string artistName, string songName, string promptStyleUsed, string songLyricsId, string analysis)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "analyzed-lyrics-item";
        CreatedAt = DateTimeOffset.UtcNow;
        ArtistName = artistName;
        SongName = songName;
        PromptStyleUsed = promptStyleUsed;
        SongLyricsId = songLyricsId;
        Analysis = analysis;
    }

    /// <summary>
    /// The date and time when the analyzed lyrics were created.
    /// </summary>
    [Column("createdAt")]
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The name of the artist.
    /// </summary>
    [Column("artistName")]
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    /// <summary>
    /// The name of the song.
    /// </summary>
    [Column("songName")]
    [JsonPropertyName("songName")]
    public string SongName { get; set; } = null!;

    /// <summary>
    /// The style of the prompt used for the completion.
    /// </summary>
    [Column("promptStyleUsed")]
    [JsonPropertyName("promptStyleUsed")]
    public string PromptStyleUsed { get; set; } = null!;

    /// <summary>
    /// The ID of the song lyrics.
    /// </summary>
    [Column("songLyricsId")]
    [JsonPropertyName("songLyricsId")]
    public string SongLyricsId { get; set; } = null!;

    /// <summary>
    /// The analysis of the lyrics.
    /// </summary>
    [Column("analysis")]
    [JsonPropertyName("analysis")]
    public string Analysis { get; set; } = null!;
}