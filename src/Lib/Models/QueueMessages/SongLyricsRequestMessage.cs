namespace MuzakBot.Lib.Models.QueueMessages;

public sealed class SongLyricsRequestMessage
{
    /*
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    */

    [JsonPropertyName("geniusUrl")]
    public string GeniusUrl { get; set; } = null!;
}