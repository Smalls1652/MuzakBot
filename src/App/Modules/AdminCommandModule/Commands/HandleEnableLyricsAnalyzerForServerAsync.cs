using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
#if DEBUG
    private const string _enableLyricsAnalyzerForServerCommandName = "lyricsanalyzerservers-dev";
#else
    private const string _enableLyricsAnalyzerForServerCommandName = "lyricsanalyzerservers";
#endif

    /// <summary>
    /// Handles the slash command for enabling/disabling the lyrics analyzer command for a specific server.
    /// </summary>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="newState">The state to set for the server.</param>
    /// <returns></returns>
    [SlashCommand(name: _enableLyricsAnalyzerForServerCommandName, description: "Enable/disable the lyrics analyzer command for a specific server.")]
    [RequireOwner(Group = "Permission")]
    private async Task HandleEnableLyricsAnalyzerForServerAsync(
        [Summary(name: "guildId", description: "The ID of the guild")]
        string guildId,
        [Summary(name: "newState", description: "Whether to enable or disable the lyrics analyzer command"),
        Choice(name: "Enable", value: "enable"),
        Choice(name: "Disable", value: "disable")]
        string newState
    )
    {
        await DeferAsync(
            ephemeral: false
        );

        ulong guildIdUlong;
        try
        {
            guildIdUlong = ulong.Parse(guildId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            await FollowupAsync(
                text: $"An error occurred while trying to parse the guild ID:\n\n`{ex.Message}`"
            );

            return;
        }

        _logger.LogInformation("Getting guild...");
        SocketGuild guild;
        try
        {
            guild = _discordSocketClient.GetGuild(guildIdUlong);

            if (guild is null)
            {
                throw new Exception($"Guild with ID '{guildIdUlong}' not found. The bot is most likely not in the guild.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            await FollowupAsync(
                text: $"An error occurred while trying to get the guild:\n\n`{ex.Message}`"
            );

            return;
        }

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
                text: "An error occurred while trying to get the lyrics analyzer config."
            );

            return;
        }

        _logger.LogInformation("Lyrics analyzer config retrieved.");

        bool guildIsEnabled = lyricsAnalyzerConfig.CommandEnabledGuildIds?.Contains(guildIdUlong) ?? false;
        bool guildIsDisabled = lyricsAnalyzerConfig.CommandDisabledGuildIds?.Contains(guildIdUlong) ?? false;

        if (newState == "enable")
        {
            if (lyricsAnalyzerConfig.CommandEnabledGuildIds is null)
            {
                lyricsAnalyzerConfig.CommandEnabledGuildIds = new();
            }

            if (guildIsEnabled)
            {
                await FollowupAsync(
                    text: $"The lyrics analyzer command is already enabled for **{guild.Name}**."
                );

                return;
            }

            if (guildIsDisabled)
            {
                lyricsAnalyzerConfig.CommandDisabledGuildIds?.Remove(guildIdUlong);
            }

            lyricsAnalyzerConfig.CommandEnabledGuildIds.Add(guildIdUlong);
        }

        if (newState == "disable")
        {
            if (lyricsAnalyzerConfig.CommandDisabledGuildIds is null)
            {
                lyricsAnalyzerConfig.CommandDisabledGuildIds = new();
            }

            if (guildIsDisabled)
            {
                await FollowupAsync(
                    text: $"The lyrics analyzer command is already disabled for **{guild.Name}**."
                );

                return;
            }

            if (guildIsEnabled)
            {
                lyricsAnalyzerConfig.CommandEnabledGuildIds?.Remove(guildIdUlong);
            }

            lyricsAnalyzerConfig.CommandDisabledGuildIds.Add(guildIdUlong);
        }

        _logger.LogInformation("Updating lyrics analyzer config...");
        try
        {
            await _cosmosDbService.AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            await FollowupAsync(
                text: "An error occurred while trying to update the lyrics analyzer config."
            );

            return;
        }

        _logger.LogInformation("Lyrics analyzer config updated.");

        try
        {
            string enabledOrDisabledString = newState == "enable" ? "enabled" : "disabled";
            await FollowupAsync(
                text: $"The lyrics analyzer command has been {enabledOrDisabledString} for **{guild.Name}**."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            await FollowupAsync(
                text: $"An error occurred while trying to send the followup message:\n\n`{ex.Message}`"
            );
        }
    }
}