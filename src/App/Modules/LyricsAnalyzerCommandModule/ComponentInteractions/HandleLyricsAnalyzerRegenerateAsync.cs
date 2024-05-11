using System.Diagnostics;
using System.Text;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Models.Responses;
using MuzakBot.App.Preconditions.LyricsAnalyzer;
using MuzakBot.Lib;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Regenerates a lyrics analyzer response.
    /// </summary>
    /// <param name="responseId">The ID of the response to regenerate.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Thrown when the lyrics analyzer item is not found in the database.</exception>
    [RequireUserRateLimit]
    [RequireLyricsAnalyzerEnabledForServer]
    [ComponentInteraction("lyrics-analyzer-regenerate-*", true)]
    private async Task HandleLyricsAnalyzerRegenerateAsync(string responseId)
    {
        using var activity = _activitySource.StartActivity(
            name: "HandleLyricsAnalyzerRegenerateAsync",
            kind: ActivityKind.Client,
            tags: new ActivityTagsCollection
            {
                { "responseId", responseId }
            }
        );

        var componentInteraction = (Context.Interaction as IComponentInteraction)!;
        bool isEphemeral = false;
        if (componentInteraction.Message.Flags is not null)
        {
            isEphemeral = ((int)componentInteraction.Message.Flags.Value & (int)MessageFlags.Ephemeral) == (int)MessageFlags.Ephemeral;
        }

        await componentInteraction.DeferLoadingAsync(isEphemeral);

        using var dbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext();

        _logger.LogInformation("Regenerating lyrics analyzer response for {responseId}", responseId);

        // Get the lyrics analyzer config from the database.
        _logger.LogInformation("Getting lyrics analyzer config from database.");
        LyricsAnalyzerConfig lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await dbContext.LyricsAnalyzerConfigs
                .WithPartitionKey("lyricsanalyzer-config")
                .FirstAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyrics analyzer config from database.");
            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Get the lyrics analyzer item from the database.
        _logger.LogInformation("Getting lyrics analyzer item from database.");
        LyricsAnalyzerItem? lyricsAnalyzerItem;
        try
        {
            lyricsAnalyzerItem = await dbContext.LyricsAnalyzerItems
                .FirstOrDefaultAsync(item => item.Id == responseId);

            if (lyricsAnalyzerItem is null)
            {
                throw new NullReferenceException("Lyrics analyzer item not found in database.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyrics analyzer item from database.");
            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Get the prompt style from the database.
        LyricsAnalyzerPromptStyle promptStyle;
        try
        {
            promptStyle = await GetPromptStyleAsync(lyricsAnalyzerItem.PromptStyle, activity?.Id);
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "Error getting prompt style '{PromptMode}' from database.", lyricsAnalyzerItem.PromptStyle);

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("The requested prompt style does not exist. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prompt style '{PromptMode}' from database.", lyricsAnalyzerItem.PromptStyle);

            await FollowupAsync(
                embed: GenerateErrorEmbed("An unknown error occurred while getting the prompt style. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Get the song lyrics.
        string lyrics;
        try
        {
            lyrics = await GetSongLyricsAsync(lyricsAnalyzerItem.ArtistName, lyricsAnalyzerItem.SongName, activity?.Id);
        }
        catch (LyricsAnalyzerDbException ex)
        {
            _logger.LogError(ex, "Error getting song lyrics for '{SongName}' by '{ArtistName}' from the database.", lyricsAnalyzerItem.SongName, lyricsAnalyzerItem.ArtistName);

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (GeniusApiException ex)
        {
            _logger.LogError(ex, "Error getting song lyrics for '{SongName}' by '{ArtistName}' from Genius.", lyricsAnalyzerItem.SongName, lyricsAnalyzerItem.ArtistName);

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("Could not find lyrics for the song on Genius. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (SongRequestJobException ex)
        {
            _logger.LogError(ex, "Error getting song lyrics for '{SongName}' by '{ArtistName}'.", lyricsAnalyzerItem.SongName, lyricsAnalyzerItem.ArtistName);

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred while getting song lyrics for '{SongName}' by '{ArtistName}'.", lyricsAnalyzerItem.SongName, lyricsAnalyzerItem.ArtistName);

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An unknown error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Analyze the lyrics with OpenAI.
        _logger.LogInformation("Analyzing lyrics for '{SongName}' by '{ArtistName}' with OpenAI.", lyricsAnalyzerItem.SongName, lyricsAnalyzerItem.ArtistName);
        OpenAiChatCompletion openAiChatCompletion;
        try
        {
            openAiChatCompletion = await RunLyricsAnalysisAsync(lyricsAnalyzerItem.ArtistName, lyricsAnalyzerItem.SongName, lyrics, promptStyle, activity?.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyric analysis for song '{songId}'.", lyricsAnalyzerItem.SongName);

            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An unknown error occurred while analyzing the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        AnalyzedLyrics analyzedLyrics = new(
            artistName: lyricsAnalyzerItem.ArtistName,
            songName: lyricsAnalyzerItem.SongName,
            promptStyleUsed: promptStyle.ShortName,
            songLyricsId: lyricsAnalyzerItem.Id,
            analysis: openAiChatCompletion.Choices[0].Message.Content
        );

        dbContext.AnalyzedLyricsItems.Add(analyzedLyrics);
        await dbContext.SaveChangesAsync();

        // Build the response.
        LyricsAnalyzerResponse lyricsAnalyzerResponse = new(
            artistName: lyricsAnalyzerItem.ArtistName,
            songName: lyricsAnalyzerItem.SongName,
            openAiChatCompletion: openAiChatCompletion,
            promptStyle: promptStyle,
            responseId: lyricsAnalyzerItem.Id,
            analysisId: analyzedLyrics.Id
        );

        // Send the response to Discord.
        _logger.LogInformation("Sending response to Discord.");
        try
        {
            await componentInteraction.FollowupAsync(
                text: lyricsAnalyzerResponse.GenerateText(),
                embed: lyricsAnalyzerResponse.GenerateEmbed().Build(),
                components: lyricsAnalyzerResponse.GenerateComponent().Build(),
                ephemeral: isEphemeral
            );

            bool userIsOnRateLimitIgnoreList = lyricsAnalyzerConfig.RateLimitIgnoredUserIds is not null && lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Contains(Context.User.Id.ToString());
            LyricsAnalyzerUserRateLimit? lyricsAnalyzerUserRateLimit = await dbContext.LyricsAnalyzerUserRateLimits
                .WithPartitionKey("user-item")
                .FirstOrDefaultAsync(item => item.UserId == Context.User.Id.ToString());

            if (lyricsAnalyzerUserRateLimit is null)
            {
                lyricsAnalyzerUserRateLimit = new(Context.User.Id.ToString());

                dbContext.LyricsAnalyzerUserRateLimits.Add(lyricsAnalyzerUserRateLimit);

                await dbContext.SaveChangesAsync();
            }

            if (lyricsAnalyzerConfig.RateLimitEnabled && !userIsOnRateLimitIgnoreList)
            {
                _logger.LogInformation("Updating rate limit for user '{UserId}' in database.", Context.User.Id);
                lyricsAnalyzerUserRateLimit!.IncrementRequestCount();
                lyricsAnalyzerUserRateLimit!.LastRequestTime = DateTimeOffset.UtcNow;

                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending response to Discord.");
            await componentInteraction.FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while sending the response. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isEphemeral
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
    }
}
