using System.Text;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    /// <summary>
    /// Handles the component interaction for the "Show enabled servers" button.
    /// </summary>
    /// <param name="componentId">The unique ID of the component.</param>
    /// <returns></returns>
    [ComponentInteraction(customId: "lyricsanalyzer-showenabledservers-*")]
    public async Task HandleShowEnabledServersButtonClickAsync(
        string componentId
    )
    {
        await DeferAsync(
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

            await FollowupAsync(
                text: "An error occurred while getting the lyrics analyzer config.",
                ephemeral: true
            );

            return;
        }

        _logger.LogInformation("Lyrics analyzer config retrieved.");

        SocketGuild[] guilds = _discordSocketClient.Guilds.ToArray();

        List<SocketGuild> enabledGuilds = [];
        List<SocketGuild> disabledGuilds = [];

        foreach (SocketGuild guild in guilds)
        {
            if (lyricsAnalyzerConfig.CommandEnabledGuildIds?.Contains(guild.Id) ?? false)
            {
                enabledGuilds.Add(guild);
            }
            
            if (lyricsAnalyzerConfig.CommandDisabledGuildIds?.Contains(guild.Id) ?? false)
            {
                disabledGuilds.Add(guild);
            }
        }

        StringBuilder responseBuilder = new("# MuzakBot - Lyrics Analyzer Admin - Enabled Servers\n\n");

        responseBuilder.AppendLine("## Enabled Servers\n");
        if (enabledGuilds.Count == 0)
        {
            responseBuilder.AppendLine("- No servers are currently enabled.");
        }
        else
        {
            foreach (SocketGuild guild in enabledGuilds)
            {
                responseBuilder.AppendLine($"- ✅ **{guild.Name}** [`{guild.Id}`]");
            }
        }

        responseBuilder.AppendLine("## Disabled Servers\n");
        if (disabledGuilds.Count == 0)
        {
            responseBuilder.AppendLine("- No servers are currently disabled.");
        }
        else
        {
            foreach (SocketGuild guild in disabledGuilds)
            {
                responseBuilder.AppendLine($"- ❌ **{guild.Name}** [`{guild.Id}`]");
            }
        }

        _logger.LogInformation("Sending message...");
        try
        {
            await FollowupAsync(
                text: responseBuilder.ToString(),
                ephemeral: true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            await FollowupAsync(
                text: "An error occurred while trying to show the lyrics analyzer admin config."
            );

            return;
        }
    }
}