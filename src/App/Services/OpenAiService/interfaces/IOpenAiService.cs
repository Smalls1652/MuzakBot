using MuzakBot.App.Models.OpenAi;

namespace MuzakBot.App.Services;

public interface IOpenAiService : IDisposable
{
    Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics);
    Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, string? parentActivityId);
}