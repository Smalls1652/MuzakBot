using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    /// <summary>
    /// Handles the component interaction for the "Modify rate limit max requests" selection.
    /// </summary>
    /// <param name="componentId">The unique ID of the component.</param>
    /// <param name="selectedValue">The selected value of the component.</param>
    /// <returns></returns>
    [ComponentInteraction(customId: "lyricsanalyzer-ratelimitmaxrequests-*")]
    public async Task HandleModifyRateLimitMaxRequestsSelectionAsync(
        string componentId,
        string selectedValue
    )
    {
        await Context.Interaction.DeferAsync(
            ephemeral: true
        );

        _logger.LogInformation("Getting lyrics analyzer config...");
        LyricsAnalyzerConfig lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await _cosmosDbService.GetLyricsAnalyzerConfigAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);

            return;
        }

        _logger.LogInformation("Lyrics analyzer config retrieved.");

        lyricsAnalyzerConfig.RateLimitMaxRequests = int.Parse(selectedValue);

        _logger.LogInformation("Updating lyrics analyzer config...");
        try
        {
            await _cosmosDbService.AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig);
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