namespace MuzakBot.App.Models.Genius;

public class GeniusApiResponse<T> : IGeniusApiResponse<T>
{
    [JsonPropertyName("meta")]
    public GeniusMeta Meta { get; set; } = new();

    [JsonPropertyName("response")]
    public T? Response { get; set; }
}