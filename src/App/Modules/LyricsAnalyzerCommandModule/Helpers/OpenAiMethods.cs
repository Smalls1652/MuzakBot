using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Runs the lyrics analysis process.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="lyrics">The lyrics of the song.</param>
    /// <param name="promptStyle">The prompt style to use.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The result of the lyrics analysis process.</returns>
    /// <exception cref="NullReferenceException">The response from the OpenAI API was null.</exception>
    private async Task<OpenAiChatCompletion> RunLyricsAnalysisAsync(string artistName, string songName, string lyrics, LyricsAnalyzerPromptStyle promptStyle, string? parentActivityId)
    {
        OpenAiChatCompletion? openAiChatCompletion;
        try
        {
            openAiChatCompletion = await _openAiService.GetLyricAnalysisAsync(
                artistName: artistName,
                songName: songName,
                lyrics: lyrics,
                promptStyle: promptStyle,
                parentActivityId: parentActivityId
            );
        }
        catch (Exception)
        {
            throw;
        }

        if (openAiChatCompletion is null || openAiChatCompletion.Choices is null || openAiChatCompletion.Choices.Length == 0)
        {
            throw new NullReferenceException("The response from the OpenAI API was null.");
        }

        return openAiChatCompletion;
    }
}
