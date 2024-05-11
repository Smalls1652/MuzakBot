namespace MuzakBot.Lib.Models.OpenAi;

/// <summary>
/// Represents a request for chat completion using the OpenAI API.
/// </summary>
public class OpenAiChatCompletionRequest : IOpenAiChatCompletionRequest
{
    /// <summary>
    /// The model to use for chat completion.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    /// <summary>
    /// The messages to use for chat completion.
    /// </summary>
    [JsonPropertyName("messages")]
    public ChatMessage[] Messages { get; set; } = null!;

    /// <summary>
    /// The temperature for chat completion. Higher values result in more random outputs.
    /// </summary>
    [JsonPropertyName("temperature")]
    public int Temperature { get; set; } = 1;

    /// <summary>
    /// The maximum number of tokens to generate for chat completion.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; } = 512;

    /// <summary>
    /// The top-p value for nucleus sampling. Controls the diversity of the output.
    /// </summary>
    [JsonPropertyName("top_p")]
    public int TopP { get; set; } = 1;

    /// <summary>
    /// The frequency penalty for chat completion. Higher values discourage repetitive responses.
    /// </summary>
    [JsonPropertyName("frequency_penalty")]
    public int FrequencyPenalty { get; set; } = 0;

    /// <summary>
    /// The presence penalty for chat completion. Higher values discourage generic responses.
    /// </summary>
    [JsonPropertyName("presence_penalty")]
    public int PresencePenalty { get; set; } = 0;
}
