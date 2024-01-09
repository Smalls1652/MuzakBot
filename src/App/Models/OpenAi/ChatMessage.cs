namespace MuzakBot.App.Models.OpenAi;

public class ChatMessage : IChatMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}