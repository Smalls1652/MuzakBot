namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public interface ISongLyricsItem
{
    string Id { get; set; }
    string PartitionKey { get; set; }
    string ArtistName { get; set; }
    string SongName { get; set; }
    string Lyrics { get; set; }
    DateTimeOffset CreatedAt { get; set; }
}