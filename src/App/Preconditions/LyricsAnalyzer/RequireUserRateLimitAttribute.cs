using System.Text;

using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MuzakBot.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Preconditions.LyricsAnalyzer;

/// <summary>
/// Precondition attribute to check if the user has reached the rate limit for the '/lyricsanalyzer' command.
/// </summary>
public class RequireUserRateLimitAttribute : PreconditionAttribute
{
    /// <inheritdoc />
    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        var lyricsAnalyzerDbContextFactory = services.GetRequiredService<IDbContextFactory<LyricsAnalyzerDbContext>>();
        var logger = services.GetRequiredService<ILogger<RequireUserRateLimitAttribute>>();

        using var dbContext = lyricsAnalyzerDbContextFactory.CreateDbContext();

        LyricsAnalyzerConfig lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await dbContext.LyricsAnalyzerConfigs
                .WithPartitionKey("lyricsanalyzer-config")
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

        // Check if the current user is on the rate limit ignore list.
        bool userIsOnRateLimitIgnoreList = lyricsAnalyzerConfig.RateLimitIgnoredUserIds is not null && lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Contains(context.User.Id.ToString());

        // Get the current rate limit for the user.
        LyricsAnalyzerUserRateLimit? lyricsAnalyzerUserRateLimit = null;
        if (lyricsAnalyzerConfig.RateLimitEnabled && !userIsOnRateLimitIgnoreList)
        {
            logger.LogInformation("Getting current rate limit for user '{UserId}' from database.", context.User.Id);
            lyricsAnalyzerUserRateLimit = await dbContext.LyricsAnalyzerUserRateLimits
                .WithPartitionKey("user-item")
                .FirstOrDefaultAsync(item => item.UserId == context.User.Id.ToString());

            if (lyricsAnalyzerUserRateLimit is null)
            {
                lyricsAnalyzerUserRateLimit = new(context.User.Id.ToString());

                dbContext.LyricsAnalyzerUserRateLimits.Add(lyricsAnalyzerUserRateLimit);

                await dbContext.SaveChangesAsync();
            }

            logger.LogInformation("Current rate limit for user '{UserId}' is {CurrentRequestCount}/{MaxRequests}.", context.User.Id, lyricsAnalyzerUserRateLimit.CurrentRequestCount, lyricsAnalyzerConfig.RateLimitMaxRequests);

            // If the user has reached the rate limit, send a message and return.
            if (!lyricsAnalyzerUserRateLimit.EvaluateRequest(lyricsAnalyzerConfig.RateLimitMaxRequests))
            {
                logger.LogInformation("User '{UserId}' has reached the rate limit.", context.User.Id);

                StringBuilder rateLimitMessageBuilder = new($"You have reached the rate limit ({lyricsAnalyzerConfig.RateLimitMaxRequests} requests) for this command. 😥\n\n");

                DateTimeOffset resetTime = lyricsAnalyzerUserRateLimit.LastRequestTime.AddDays(1);

                rateLimitMessageBuilder.AppendLine($"Rate limit will reset <t:{resetTime.ToUnixTimeSeconds()}:R>.");

                EmbedBuilder rateLimitExceededEmbed = new EmbedBuilder()
                    .WithTitle("💥 An error occurred")
                    .WithDescription(rateLimitMessageBuilder.ToString())
                    .WithColor(Color.Red);

                await context.Interaction.RespondAsync(
                    embed: rateLimitExceededEmbed.Build(),
                    ephemeral: true
                );

                return PreconditionResult.FromError("User has reached the rate limit for this command.");
            }
        }

        return PreconditionResult.FromSuccess();
    }
}