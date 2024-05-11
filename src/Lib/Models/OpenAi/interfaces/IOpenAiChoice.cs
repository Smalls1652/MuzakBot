namespace MuzakBot.Lib.Models.OpenAi;

public interface IOpenAiChoice
{
    int Index { get; set; }
    string FinishReason { get; set; }
    ChatMessage Message { get; set; }
}
