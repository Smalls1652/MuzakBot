using System.Diagnostics;
using System.Text;

using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
using MuzakBot.App.Models.Responses;
using MuzakBot.App.Services;
using MuzakBot.Lib;
using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.Genius;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.Lib.Models.OpenAi;
using MuzakBot.Lib.Models.QueueMessages;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Handles the 'lyricsanalyzer' slash command.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="promptMode">The style of the response.</param>
    /// <param name="isPrivateResponse">Whether or not to send the response privately.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [SlashCommand(
        name: "lyricsanalyzer",
        description: "Get an analysis of the lyrics of a song"
    )]
    private async Task LyricsAnalyzerCommandAsync(
        [Summary("artistName", "The name of an artist"),
         Autocomplete(typeof(AppleMusicArtistAutoCompleteHandler))
        ]
        string artistId,
        [Summary(name: "songName", description: "The name of a song"),
         Autocomplete(typeof(AppleMusicArtistSongAutocompleteHandler))
        ]
        string songId,
        [Summary(name: "style", description: "The style of the response"),
        Autocomplete(typeof(LyricsAnalyzerPromptStyleAutoCompleteHandler))]
        string promptMode = "normal",
        [Summary(name: "private", description: "Whether or not to send the response privately")]
        bool isPrivateResponse = false
    )
    {
        using var activity = _activitySource.StartLyricsAnalyzerCommandAsyncActivity(artistId, songId, Context);

        await DeferAsync(
            ephemeral: isPrivateResponse
        );

        using var dbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext();

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
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Check if the command is enabled for the current server.
        if (!Context.Interaction.IsDMInteraction && lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds && lyricsAnalyzerConfig.CommandEnabledGuildIds is not null && !lyricsAnalyzerConfig.CommandEnabledGuildIds.Contains(Context.Guild.Id))
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("This command is not enabled on this server. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            return;
        }

        // Check if the command is disabled for the current server.
        if (!Context.Interaction.IsDMInteraction && !lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds && lyricsAnalyzerConfig.CommandDisabledGuildIds is not null && lyricsAnalyzerConfig.CommandDisabledGuildIds.Contains(Context.Guild.Id))
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("This command is not enabled on this server. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            return;
        }

        // Check if the current user is on the rate limit ignore list.
        bool userIsOnRateLimitIgnoreList = lyricsAnalyzerConfig.RateLimitIgnoredUserIds is not null && lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Contains(Context.User.Id.ToString());

        // Get the current rate limit for the user.
        LyricsAnalyzerUserRateLimit? lyricsAnalyzerUserRateLimit = null;
        if (lyricsAnalyzerConfig.RateLimitEnabled && !userIsOnRateLimitIgnoreList)
        {
            _logger.LogInformation("Getting current rate limit for user '{UserId}' from database.", Context.User.Id);
            lyricsAnalyzerUserRateLimit = await dbContext.LyricsAnalyzerUserRateLimits
                .WithPartitionKey("user-item")
                .FirstOrDefaultAsync(item => item.UserId == Context.User.Id.ToString());

            if (lyricsAnalyzerUserRateLimit is null)
            {
                lyricsAnalyzerUserRateLimit = new(Context.User.Id.ToString());

                dbContext.LyricsAnalyzerUserRateLimits.Add(lyricsAnalyzerUserRateLimit);

                await dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Current rate limit for user '{UserId}' is {CurrentRequestCount}/{MaxRequests}.", Context.User.Id, lyricsAnalyzerUserRateLimit.CurrentRequestCount, lyricsAnalyzerConfig.RateLimitMaxRequests);

            // If the user has reached the rate limit, send a message and return.
            if (!lyricsAnalyzerUserRateLimit.EvaluateRequest(lyricsAnalyzerConfig.RateLimitMaxRequests))
            {
                _logger.LogInformation("User '{UserId}' has reached the rate limit.", Context.User.Id);

                StringBuilder rateLimitMessageBuilder = new($"You have reached the rate limit ({lyricsAnalyzerConfig.RateLimitMaxRequests} requests) for this command. 😥\n\n");

                DateTimeOffset resetTime = lyricsAnalyzerUserRateLimit.LastRequestTime.AddDays(1);

                rateLimitMessageBuilder.AppendLine($"Rate limit will reset <t:{resetTime.ToUnixTimeSeconds()}:R>.");

                await FollowupAsync(
                    embed: GenerateErrorEmbed(rateLimitMessageBuilder.ToString()).Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: isPrivateResponse
                );

                return;
            }
        }

        // Get the prompt style from the database.
        LyricsAnalyzerPromptStyle promptStyle;
        try
        {
            promptStyle = await dbContext.LyricsAnalyzerPromptStyles
                .WithPartitionKey("prompt-style")
                .FirstAsync(item => item.ShortName == promptMode);
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "Error getting prompt style '{PromptMode}' from database.", promptMode);

            await FollowupAsync(
                embed: GenerateErrorEmbed("The requested prompt style does not exist. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prompt style '{PromptMode}' from database.", promptMode);

            await FollowupAsync(
                embed: GenerateErrorEmbed("An unknown error occurred while getting the prompt style. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Get data about the artist from the Apple Music API.
        Artist artist;
        try
        {
            artist = await _appleMusicApiService.GetArtistFromCatalogAsync(artistId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting artist '{artistId}'.", artistId);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the artist. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Get data about the song from the Apple Music API.
        Song song;
        try
        {
            song = await _appleMusicApiService.GetSongFromCatalogAsync(songId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song '{songId}'.", songId);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the song. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        string artistName = artist.Attributes!.Name;
        string songName = song.Attributes!.Name;

        // Get the song lyrics.
        string lyrics;
        try
        {
            lyrics = await GetSongLyricsAsync(artistName, songName, activity?.Id);
        }
        catch (LyricsAnalyzerDbException ex)
        {
            _logger.LogError(ex, "Error getting song lyrics for '{SongName}' by '{ArtistName}' from the database.", songName, artistName);

            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (GeniusApiException ex)
        {
            _logger.LogError(ex, "Error getting song lyrics for '{SongName}' by '{ArtistName}' from Genius.", songName, artistName);

            await FollowupAsync(
                embed: GenerateErrorEmbed("Could not find lyrics for the song on Genius. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (SongRequestJobException ex)
        {
            _logger.LogError(ex, "Error getting song lyrics for '{SongName}' by '{ArtistName}'.", songName, artistName);

            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred while getting song lyrics for '{SongName}' by '{ArtistName}'.", songName, artistName);

            await FollowupAsync(
                embed: GenerateErrorEmbed("An unknown error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        // Analyze the lyrics with OpenAI.
        _logger.LogInformation("Analyzing lyrics for '{SongName}' by '{ArtistName}' with OpenAI.", songName, artistName);
        OpenAiChatCompletion openAiChatCompletion;
        try
        {
            openAiChatCompletion = await _openAiService.GetLyricAnalysisAsync(
                artistName: artistName,
                songName: songName,
                lyrics: lyrics,
                promptStyle: promptStyle,
                parentActivityId: activity?.Id
            ) ?? throw new NullReferenceException("The response from the OpenAI API was null.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyric analysis for song '{songId}'.", songName);

            await FollowupAsync(
                embed: GenerateErrorEmbed("An unknown error occurred while analyzing the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        LyricsAnalyzerItem lyricsAnalyzerItem = new(
            artistName: artistName,
            songName: songName,
            promptStyle: promptStyle.ShortName
        );

        dbContext.LyricsAnalyzerItems.Add(lyricsAnalyzerItem);

        await dbContext.SaveChangesAsync();

        // Build the response.
        LyricsAnalyzerResponse lyricsAnalyzerResponse = new(
            artistName: artistName,
            songName: songName,
            openAiChatCompletion: openAiChatCompletion,
            promptStyle: promptStyle,
            responseId: lyricsAnalyzerItem.Id
        );

        // Send the response to Discord.
        _logger.LogInformation("Sending response to Discord.");
        try
        {
            await FollowupAsync(
                text: lyricsAnalyzerResponse.GenerateText(),
                embed: lyricsAnalyzerResponse.GenerateEmbed().Build(),
                components: lyricsAnalyzerResponse.GenerateComponent().Build(),
                ephemeral: isPrivateResponse
            );

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
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while sending the response. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
    }
}