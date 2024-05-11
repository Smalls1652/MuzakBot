using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Interface for the OpenAI service.
/// </summary>
public interface IOpenAiService : IDisposable
{
    Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, LyricsAnalyzerPromptStyle promptStyle);
    Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, LyricsAnalyzerPromptStyle promptStyle, string? parentActivityId);
}
