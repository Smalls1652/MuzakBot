namespace MuzakBot.Lib.Models.OpenAi;

public interface IOpenAiChatCompletionRequest
{
    string Model { get; set; }
    ChatMessage[] Messages { get; set; }
    int Temperature { get; set; }
    int MaxTokens { get; set; }
    int TopP { get; set; }
    int FrequencyPenalty { get; set; }
    int PresencePenalty { get; set; }
}