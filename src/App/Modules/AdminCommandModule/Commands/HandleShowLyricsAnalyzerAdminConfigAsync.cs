using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
#if DEBUG
    private const string _lyricsAnalyzerAdminCommandName = "lyricsanalyzer-admin-dev";
#else
    private const string _lyricsAnalyzerAdminCommandName = "lyricsanalyzer-admin";
#endif

    /// <summary>
    /// Handles the slash command for the lyrics analyzer admin command.
    /// </summary>
    /// <returns></returns>
    [SlashCommand(name: _lyricsAnalyzerAdminCommandName, description: "Configuration utility for the lyrics analyzer command.")]
    [RequireOwner(Group = "Permission")]
    public async Task HandleShowLyricsAnalyzerAdminConfigAsync()
    {
        await DeferAsync(
            ephemeral: true
        );

        _logger.LogInformation("Generating component...");
        ComponentBuilder componentBuilder;
        try
        {
            componentBuilder = await GenerateAdminConfigComponentAsync(
                currentGuildId: Context.Guild.Id,
                componentId: null
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

        _logger.LogInformation("Sending message...");
        try
        {
            await FollowupAsync(
                text: "# MuzakBot - Lyrics Analyzer Admin",
                components: componentBuilder.Build()
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