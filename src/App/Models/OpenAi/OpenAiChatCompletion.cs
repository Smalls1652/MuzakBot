namespace MuzakBot.App.Models.OpenAi;

public class OpenAiChatCompletion : IOpenAiChatCompletion
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("object")]
    public string Object { get; set; } = null!;

    [JsonPropertyName("created")]
    public int Created { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; } = null!;

    [JsonPropertyName("choices")]
    public OpenAiChoice[] Choices { get; set; } = null!;

    [JsonPropertyName("system")]
    public string SystemFingerprint { get; set; } = null!;

    [JsonPropertyName("completion")]
    public OpenAiUsage Usage { get; set; } = null!;
}