using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    /// <summary>
    /// Handles the component interaction for the "Enable rate limit" button.
    /// </summary>
    /// <param name="componentId">The unique ID of the component.</param>
    /// <returns></returns>
    [ComponentInteraction(customId: "lyricsanalyzer-ratelimitenabled-*")]
    public async Task HandleEnableRateLimitButtonClickAsync(
        string componentId
    )
    {
        await Context.Interaction.DeferAsync(
            ephemeral: true
        );

        using var lyricsAnalyzerContext = _lyricsAnalyzerContextFactory.CreateDbContext();

        _logger.LogInformation("Getting lyrics analyzer config...");
        LyricsAnalyzerConfig lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await lyricsAnalyzerContext.Configs.FirstOrDefaultAsync() ?? throw new InvalidOperationException("Lyrics analyzer config not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);

            return;
        }

        _logger.LogInformation("Lyrics analyzer config retrieved.");

        lyricsAnalyzerConfig.RateLimitEnabled = !lyricsAnalyzerConfig.RateLimitEnabled;

        _logger.LogInformation("Updating lyrics analyzer config...");
        try
        {
            lyricsAnalyzerContext.Configs.Update(lyricsAnalyzerConfig);
            await lyricsAnalyzerContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);

            return;
        }

        _logger.LogInformation("Lyrics analyzer config updated.");

        _logger.LogInformation("Generating component...");
        ComponentBuilder componentBuilder;
        try
        {
            componentBuilder = await GenerateAdminConfigComponentAsync(
                currentGuildId: Context.Guild.Id,
                componentId: componentId
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);

            return;
        }

        _logger.LogInformation("Modifying message...");
        try
        {
            await Context.Interaction.ModifyOriginalResponseAsync(
                messageProps => messageProps.Components = componentBuilder.Build()
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);

            return;
        }
    }
}