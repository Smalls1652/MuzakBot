namespace MuzakBot.App.Models.Genius;

public class GeniusMeta : IGeniusMeta
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
}