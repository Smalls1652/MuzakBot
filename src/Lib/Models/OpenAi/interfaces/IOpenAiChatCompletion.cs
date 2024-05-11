namespace MuzakBot.Lib.Models.OpenAi;

public interface IOpenAiChatCompletion
{
    string Id { get; set; }
    string Object { get; set; }
    int Created { get; set; }
    string Model { get; set; }
    string SystemFingerprint { get; set; }
    OpenAiChoice[] Choices { get; set; }
    OpenAiUsage Usage { get; set; }
}
