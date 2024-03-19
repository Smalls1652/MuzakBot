namespace MuzakBot.Lib.Models.OpenAi;

/// <summary>
/// Represents the token usage statistics of the OpenAI API call.
/// </summary>
public class OpenAiUsage : IOpenAiUsage
{
    /// <summary>
    /// The number of tokens used for completion (Generation).
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// The number of tokens used for prompts (Input).
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// The total number of tokens used.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}