namespace MuzakBot.Lib.Models.OpenAi;

/// <summary>
/// Represents a chat message in the MuzakBot application.
/// </summary>
public class ChatMessage : IChatMessage
{
    /// <summary>
    /// The role of the chat message.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    /// <summary>
    /// The name associated with the chat message.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The content of the chat message.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}