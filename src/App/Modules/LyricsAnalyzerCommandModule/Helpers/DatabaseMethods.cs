using Microsoft.EntityFrameworkCore;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Gets the prompt style from the database.
    /// </summary>
    /// <param name="promptMode">The short name of the prompt style.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The prompt style.</returns>
    /// <exception cref="NullReferenceException">The prompt style was null.</exception>
    private async Task<LyricsAnalyzerPromptStyle> GetPromptStyleAsync(string promptMode, string? parentActivityId)
    {
        using var dbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext();

        LyricsAnalyzerPromptStyle? promptStyle;
        try
        {
            promptStyle = await dbContext.LyricsAnalyzerPromptStyles
                .WithPartitionKey("prompt-style")
                .FirstAsync(item => item.ShortName == promptMode);
        }
        catch (Exception)
        {
            throw;
        }

        if (promptStyle is null)
        {
            throw new NullReferenceException("The prompt style was null.");
        }

        return promptStyle;
    }
}
