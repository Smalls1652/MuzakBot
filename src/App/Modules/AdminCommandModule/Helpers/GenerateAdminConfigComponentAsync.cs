using Discord;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    /// <summary>
    /// Generates the admin config component for the lyrics analyzer command.
    /// </summary>
    /// <param name="currentGuildId">The current guild ID.</param>
    /// <param name="componentId">The unique ID of the component.</param>
    /// <returns></returns>
    public async Task<ComponentBuilder> GenerateAdminConfigComponentAsync(ulong currentGuildId, string? componentId)
    {
        using var dbContext = _muzakbotDbContextFactory.CreateDbContext();

        string uniqueId = componentId ?? Guid.NewGuid().ToString().Split('-').First();

        _logger.LogInformation("Getting lyrics analyzer config...");
        LyricsAnalyzerConfig? lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await dbContext.LyricsAnalyzerConfigs
                .WithPartitionKey("lyricsanalyzer-config")
                .FirstOrDefaultAsync();

            if (lyricsAnalyzerConfig is null)
            {
                lyricsAnalyzerConfig = new(true);

                dbContext.LyricsAnalyzerConfigs.Add(lyricsAnalyzerConfig);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception)
        {
            throw;
        }

        _logger.LogInformation("Lyrics analyzer config retrieved.");

        ButtonBuilder enabledForAllButtonBuilder = new ButtonBuilder()
            .WithCustomId($"lyricsanalyzer-enabled-{uniqueId}")
            .WithLabel(!lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds ? "Enabled for all servers" : "Enabled for specific servers")
            .WithEmote(!lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds ? new Emoji("✅") : new Emoji("⚠️"))
            .WithStyle(!lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds ? ButtonStyle.Success : ButtonStyle.Secondary);

        ButtonBuilder showEnabledServersButtonBuilder = new ButtonBuilder()
            .WithCustomId($"lyricsanalyzer-showenabledservers-{uniqueId}")
            .WithLabel("Show enabled servers")
            .WithEmote(new Emoji("📃"))
            .WithStyle(ButtonStyle.Secondary);

        ButtonBuilder rateLimitEnabledButtonBuilder = new ButtonBuilder()
            .WithCustomId($"lyricsanalyzer-ratelimitenabled-{uniqueId}")
            .WithLabel(lyricsAnalyzerConfig.RateLimitEnabled ? "Rate limit enabled" : "Rate limit disabled")
            .WithEmote(lyricsAnalyzerConfig.RateLimitEnabled ? new Emoji("✅") : new Emoji("🚫"))
            .WithStyle(lyricsAnalyzerConfig.RateLimitEnabled ? ButtonStyle.Success : ButtonStyle.Secondary);

        List<SelectMenuOptionBuilder> rateLimitChangeMaxRequestSelectOptionsBuilder = new();
        for (int i = 0; i <= 100; i = i + 5)
        {
            rateLimitChangeMaxRequestSelectOptionsBuilder.Add(
                new SelectMenuOptionBuilder()
                    .WithLabel($"Rate limit: {i} max requests")
                    .WithValue(i.ToString())
                    .WithDefault(i == lyricsAnalyzerConfig.RateLimitMaxRequests)
            );
        }

        SelectMenuBuilder rateLimitChangeMaxRequestSelectBuilder = new SelectMenuBuilder()
            .WithCustomId($"lyricsanalyzer-ratelimitmaxrequests-{uniqueId}")
            .WithType(ComponentType.SelectMenu)
            .WithMinValues(1)
            .WithMaxValues(1)
            .WithOptions(rateLimitChangeMaxRequestSelectOptionsBuilder);

        ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithButton(enabledForAllButtonBuilder, 0)
            .WithButton(showEnabledServersButtonBuilder, 0)
            .WithButton(rateLimitEnabledButtonBuilder, 1)
            .WithSelectMenu(rateLimitChangeMaxRequestSelectBuilder, 2);

        return componentBuilder;
    }
}
