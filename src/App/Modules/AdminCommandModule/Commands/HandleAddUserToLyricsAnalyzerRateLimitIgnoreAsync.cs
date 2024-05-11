using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.CommandModules;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    [SlashCommand(name: "la-ratelimit-ignore", description: "Add or remove a user from the rate limit ignore list.")]
    [RequireOwner]
    public async Task HandleAddUserToLyricsAnalyzerRateLimitIgnoreAsync(
        [Summary(name: "userId", description: "The user to add to the rate limit ignore list.")]
        string userId,
        [Summary(name: "operation", description: "Whether to add or remove the user from the rate limit ignore list."),
        Choice(name: "Add", value: "add"),
        Choice(name: "Remove", value: "remove")]
        string operation = "add"
    )
    {
        await DeferAsync(
            ephemeral: true
        );

        EmbedBuilder embedBuilder = new();

        using var dbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext();

        IUser user;
        try
        {
            user = await _discordSocketClient.GetUserAsync(ulong.Parse(userId));
        }
        catch (Exception ex)
        {
            embedBuilder
                .WithTitle("Lyrics Analyzer Rate Limit Ignore")
                .WithDescription($"An error occurred while adding the user to the rate limit ignore list:\n\n{ex.Message}")
                .WithColor(Color.Red);

            await FollowupAsync(
                embeds: [
                    embedBuilder.Build()
                ]
            );

            _logger.LogError(ex, "An error occurred while adding the user to the rate limit ignore list: {Message}", ex.Message);

            return;
        }

        LyricsAnalyzerConfig? lyricsAnalyzerConfig = await dbContext.LyricsAnalyzerConfigs
            .WithPartitionKey("lyricsanalyzer-config")
            .FirstOrDefaultAsync();

        if (lyricsAnalyzerConfig is null)
        {
            lyricsAnalyzerConfig = new(true);

            dbContext.LyricsAnalyzerConfigs.Add(lyricsAnalyzerConfig);
        }

        if (lyricsAnalyzerConfig.RateLimitIgnoredUserIds is null)
        {
            _logger.LogInformation("Rate limit ignored user IDs list is null, initializing to empty list.");
            lyricsAnalyzerConfig.RateLimitIgnoredUserIds = new();
        }

        if (operation == "add")
        {
            if (lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Contains(user.Id.ToString()))
            {
                _logger.LogWarning("{Username} is already on the rate limit ignore list.", user.Username);
                embedBuilder
                    .WithTitle("Lyrics Analyzer Rate Limit Ignore")
                    .WithDescription($"The user `{user.Username}` is already on the rate limit ignore list.")
                    .WithColor(Color.Red);

                await FollowupAsync(
                    embeds: [
                        embedBuilder.Build()
                    ]
                );

                return;
            }

            lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Add(user.Id.ToString());
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("{Username} has been added to the rate limit ignore list.", user.Username);

            embedBuilder
                .WithTitle("Lyrics Analyzer Rate Limit Ignore")
                .WithDescription($"The user `{user.Username}` has been added to the rate limit ignore list.")
                .WithColor(Color.Green);

            await FollowupAsync(
                embeds: [
                    embedBuilder.Build()
                ]
            );

            return;
        }
        else if (operation == "remove")
        {
            if (!lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Contains(user.Id.ToString()))
            {
                _logger.LogWarning("{Username} is not on the rate limit ignore list.", user.Username);
                embedBuilder
                    .WithTitle("Lyrics Analyzer Rate Limit Ignore")
                    .WithDescription($"The user `{user.Username}` is not on the rate limit ignore list.")
                    .WithColor(Color.Red);

                await FollowupAsync(
                    embeds: [
                        embedBuilder.Build()
                    ]
                );

                return;
            }

            lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Remove(user.Id.ToString());
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("{Username} has been removed from the rate limit ignore list.", user.Username);

            embedBuilder
                .WithTitle("Lyrics Analyzer Rate Limit Ignore")
                .WithDescription($"The user `{user.Username}` has been removed from the rate limit ignore list.")
                .WithColor(Color.Green);

            await FollowupAsync(
                embeds: [
                    embedBuilder.Build()
                ]
            );

            return;
        }
    }
}
