using MuzakBot.App.Models.OpenAi;

namespace MuzakBot.App.Services;

/// <summary>
/// Interface for the OpenAI service.
/// </summary>
public interface IOpenAiService : IDisposable
{
    Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, string memeMode);
    Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, string memeMode, string? parentActivityId);
}