using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for a completed lyrics analyzer command run.
/// </summary>
public class LyricsAnalyzerItem : DatabaseItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerItem"/> class.
    /// </summary>
    [JsonConstructor()]
    public LyricsAnalyzerItem()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerItem"/> class.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="promptStyle">The style of the prompt used for the completion.</param>
    public LyricsAnalyzerItem(string artistName, string songName, string promptStyle)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "lyrics-analyzer-item";
        ArtistName = artistName;
        SongName = songName;
        PromptStyle = promptStyle;
        CreatedAt = DateTimeOffset.UtcNow;
    }

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
    [Column("promptStyle")]
    [JsonPropertyName("promptStyle")]
    public string PromptStyle { get; set; } = null!;

    /// <summary>
    /// The date and time the lyrics analyzer job was created.
    /// </summary>
    [Column("createdAt")]
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
