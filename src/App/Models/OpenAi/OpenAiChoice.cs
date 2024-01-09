namespace MuzakBot.App.Models.OpenAi;

public class OpenAiChoice : IOpenAiChoice
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = null!;

    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; } = null!;
}