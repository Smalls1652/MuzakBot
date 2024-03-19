namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for a song's lyrics.
/// </summary>
public class SongLyricsItem : DatabaseItem, ISongLyricsItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongLyricsItem"/> class.
    /// </summary>
    [JsonConstructor()]
    public SongLyricsItem()
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="SongLyricsItem"/> class.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="lyrics">The lyrics of the song.</param>
    public SongLyricsItem(string artistName, string songName, string lyrics)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "song-lyrics-item";
        ArtistName = artistName;
        SongName = songName;
        Lyrics = lyrics;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// The name of the artist.
    /// </summary>
    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    /// <summary>
    /// The name of the song.
    /// </summary>
    [JsonPropertyName("songName")]
    public string SongName { get; set; } = null!;

    /// <summary>
    /// The lyrics of the song.
    /// </summary>
    [JsonPropertyName("lyrics")]
    public string Lyrics { get; set; } = null!;

    /// <summary>
    /// The date and time the song lyrics were created in the database.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}