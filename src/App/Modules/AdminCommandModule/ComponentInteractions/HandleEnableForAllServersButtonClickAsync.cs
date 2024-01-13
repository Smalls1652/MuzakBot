using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    [ComponentInteraction(customId: "lyricsanalyzer-enabled-*")]
    public async Task HandleEnableForAllServersButtonClickAsync(
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

        lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds = !lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds;

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