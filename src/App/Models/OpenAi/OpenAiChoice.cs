namespace MuzakBot.App.Models.OpenAi;

/// <summary>
/// Represents a choice made by the OpenAI model.
/// </summary>
public class OpenAiChoice : IOpenAiChoice
{
    /// <summary>
    /// The index of the choice.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// The reason for finishing the choice.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = null!;

    /// <summary>
    /// The chat message associated with the choice.
    /// </summary>
    [JsonPropertyName("message")]
    public ChatMessage Message { get; set; } = null!;
}