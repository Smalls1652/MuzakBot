namespace MuzakBot.Lib.Models.OpenAi;

public interface IOpenAiUsage
{
    int CompletionTokens { get; set; }
    int PromptTokens { get; set; }
    int TotalTokens { get; set; }
}
