namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public class SongLyricsItem : DatabaseItem, ISongLyricsItem
{
    [JsonConstructor()]
    public SongLyricsItem()
    {}

    public SongLyricsItem(string artistName, string songName, string lyrics)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "song-lyrics-item";
        ArtistName = artistName;
        SongName = songName;
        Lyrics = lyrics;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    [JsonPropertyName("artistName")]
    public string ArtistName { get; set; } = null!;

    [JsonPropertyName("songName")]
    public string SongName { get; set; } = null!;

    [JsonPropertyName("lyrics")]
    public string Lyrics { get; set; } = null!;

    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }
}