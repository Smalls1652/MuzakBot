namespace MuzakBot.App.Models.OpenAi;

public class OpenAiChatCompletionRequest : IOpenAiChatCompletionRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("messages")]
    public ChatMessage[] Messages { get; set; } = null!;

    [JsonPropertyName("temperature")]
    public int Temperature { get; set; } = 1;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 512;

    [JsonPropertyName("top_p")]
    public int TopP { get; set; } = 1;

    [JsonPropertyName("frequency_penalty")]
    public int FrequencyPenalty { get; set; } = 0;

    [JsonPropertyName("presence_penalty")]
    public int PresencePenalty { get; set; } = 0;
}