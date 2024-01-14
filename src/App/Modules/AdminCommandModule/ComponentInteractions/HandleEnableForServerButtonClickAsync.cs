using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    /// <summary>
    /// Handles the component interaction for the "Enable for server" button.
    /// </summary>
    /// <remarks>
    /// This method will be removed, since the "Enable for server" button is no longer used.
    /// </remarks>
    /// <param name="componentId">The unique ID of the component.</param>
    /// <returns></returns>
    [Obsolete("This method will be removed, since the \"Enable for server\" button is no longer used.")]
    [ComponentInteraction(customId: "lyricsanalyzer-enabledforserver-*")]
    public async Task HandleEnableForServerButtonClickAsync(
        string componentId
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

        ulong currentGuildId = Context.Guild.Id;

        if (lyricsAnalyzerConfig.CommandEnabledGuildIds?.Contains(currentGuildId) ?? false)
        {
            if (lyricsAnalyzerConfig.CommandEnabledGuildIds is null || lyricsAnalyzerConfig.CommandEnabledGuildIds.Count == 0)
            {
                lyricsAnalyzerConfig.CommandEnabledGuildIds = null;
            }
            else
            {
                lyricsAnalyzerConfig.CommandEnabledGuildIds.Remove(currentGuildId);
            }
        }
        else
        {
            if (lyricsAnalyzerConfig.CommandEnabledGuildIds is null)
            {
                lyricsAnalyzerConfig.CommandEnabledGuildIds = new();
            }

            lyricsAnalyzerConfig.CommandEnabledGuildIds.Add(currentGuildId);
        }

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