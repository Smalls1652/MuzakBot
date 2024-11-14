using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MuzakBot.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Preconditions.LyricsAnalyzer;

/// <summary>
/// Precondition attribute to check if the '/lyricsanalyzer' command is enabled for the current server.
/// </summary>
public class RequireLyricsAnalyzerEnabledForServerAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var muzakbotDbContextFactory = services.GetRequiredService<IDbContextFactory<MuzakBotDbContext>>();
        var logger = services.GetRequiredService<ILogger<RequireLyricsAnalyzerEnabledForServerAttribute>>();

        using var dbContext = muzakbotDbContextFactory.CreateDbContext();

        LyricsAnalyzerConfig lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await dbContext.LyricsAnalyzerConfigs
                .FirstAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting lyrics analyzer config from database.");

            EmbedBuilder rateLimitExceededEmbed = new EmbedBuilder()
                    .WithTitle("💥 An error occurred")
                    .WithDescription("An error occurred while checking your rate limit. Please try again later. 😥")
                    .WithColor(Color.Red);

            return PreconditionResult.FromError(ex);
        }

        // Check if the command is enabled for the current server.
        if (!context.Interaction.IsDMInteraction && lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds && lyricsAnalyzerConfig.CommandEnabledGuildIds is not null && !lyricsAnalyzerConfig.CommandEnabledGuildIds.Contains(context.Guild.Id))
        {
            EmbedBuilder notEnabledEmbed = new EmbedBuilder()
                .WithTitle("💥 An error occurred")
                .WithDescription("This command is not enabled on this server. 😥")
                .WithColor(Color.Red);

            await context.Interaction.RespondAsync(
                embed: notEnabledEmbed.Build(),
                ephemeral: true
            );

            return PreconditionResult.FromError("The '/lyricsanalyzer' command is not enabled for this server.");
        }

        // Check if the command is disabled for the current server.
        if (!context.Interaction.IsDMInteraction && !lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds && lyricsAnalyzerConfig.CommandDisabledGuildIds is not null && lyricsAnalyzerConfig.CommandDisabledGuildIds.Contains(context.Guild.Id))
        {
            EmbedBuilder notEnabledEmbed = new EmbedBuilder()
                .WithTitle("💥 An error occurred")
                .WithDescription("This command is not enabled on this server. 😥")
                .WithColor(Color.Red);

            await context.Interaction.RespondAsync(
                embed: notEnabledEmbed.Build(),
                ephemeral: true
            );

            return PreconditionResult.FromError("The '/lyricsanalyzer' command is not enabled for this server.");
        }

        return PreconditionResult.FromSuccess();
    }
}
