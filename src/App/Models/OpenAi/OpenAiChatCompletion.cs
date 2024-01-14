namespace MuzakBot.App.Models.OpenAi;

/// <summary>
/// Represents the response from an OpenAI chat completion API call.
/// </summary>
public class OpenAiChatCompletion : IOpenAiChatCompletion
{
    /// <summary>
    /// The ID of the chat completion.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The object type.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = null!;

    /// <summary>
    /// The total time taken to generate the completion.
    /// </summary>
    [JsonPropertyName("created")]
    public int Created { get; set; }

    /// <summary>
    /// The model used for the completion.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    /// <summary>
    /// A collection of generated choices.
    /// </summary>
    [JsonPropertyName("choices")]
    public OpenAiChoice[] Choices { get; set; } = null!;

    /// <summary>
    /// The system fingerprint.
    /// </summary>
    [JsonPropertyName("system")]
    public string SystemFingerprint { get; set; } = null!;

    /// <summary>
    /// The token usage details of the completion.
    /// </summary>
    [JsonPropertyName("completion")]
    public OpenAiUsage Usage { get; set; } = null!;
}