using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.Genius;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.Lib.Models.OpenAi;
using MuzakBot.App.Services;

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
    [CommandContextType(InteractionContextType.Guild, InteractionContextType.PrivateChannel, InteractionContextType.BotDm)]
    [SlashCommand(
        name: "lyricsanalyzer",
        description: "Get an analysis of the lyrics of a song"
    )]
    private async Task HandleLyricsAnalyzerAsync(
        [Summary("artistName", "The name of an artist"),
         Autocomplete(typeof(ArtistNameSearchAutoCompleteHandler))
        ]
        string artistName,
        [Summary(name: "songName", description: "The name of a song"),
         Autocomplete(typeof(ArtistSongNameSearchAutoCompleteHandler))
        ]
        string songName,
        [Summary(name: "style", description: "The style of the response"),
        Autocomplete(typeof(LyricsAnalyzerPromptStyleAutoCompleteHandler))]
        string promptMode = "normal",
        [Summary(name: "private", description: "Whether or not to send the response privately")]
        bool isPrivateResponse = false
    )
    {
        using var activity = _activitySource.StartHandleLyricsAnalyzerAsyncActivity(artistName, songName, Context);

        await DeferAsync(
            ephemeral: isPrivateResponse
        );

        _logger.LogInformation("Getting lyrics analyzer config from database.");
        LyricsAnalyzerConfig lyricsAnalyzerConfig;
        try
        {
            lyricsAnalyzerConfig = await _cosmosDbService.GetLyricsAnalyzerConfigAsync(activity?.Id);
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


        if (!Context.Interaction.IsDMInteraction && lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds && lyricsAnalyzerConfig.CommandEnabledGuildIds is not null && !lyricsAnalyzerConfig.CommandEnabledGuildIds.Contains(Context.Guild.Id))
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("This command is not enabled on this server. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            return;
        }

        if (!Context.Interaction.IsDMInteraction && !lyricsAnalyzerConfig.CommandIsEnabledToSpecificGuilds && lyricsAnalyzerConfig.CommandDisabledGuildIds is not null && lyricsAnalyzerConfig.CommandDisabledGuildIds.Contains(Context.Guild.Id))
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("This command is not enabled on this server. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            return;
        }

        bool userIsOnRateLimitIgnoreList = lyricsAnalyzerConfig.RateLimitIgnoredUserIds is not null && lyricsAnalyzerConfig.RateLimitIgnoredUserIds.Contains(Context.User.Id.ToString());

        LyricsAnalyzerUserRateLimit? lyricsAnalyzerUserRateLimit = null;
        if (lyricsAnalyzerConfig.RateLimitEnabled && !userIsOnRateLimitIgnoreList)
        {
            _logger.LogInformation("Getting current rate limit for user '{UserId}' from database.", Context.User.Id);
            lyricsAnalyzerUserRateLimit = await _cosmosDbService.GetLyricsAnalyzerUserRateLimitAsync(Context.User.Id.ToString(), activity?.Id);

            _logger.LogInformation("Current rate limit for user '{UserId}' is {CurrentRequestCount}/{MaxRequests}.", Context.User.Id, lyricsAnalyzerUserRateLimit.CurrentRequestCount, lyricsAnalyzerConfig.RateLimitMaxRequests);

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

        LyricsAnalyzerPromptStyle? promptStyle;

        try
        {
            promptStyle = await _cosmosDbService.GetLyricsAnalyzerPromptStyleAsync(promptMode, activity?.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting prompt style from database.");
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the prompt style. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (promptStyle is null)
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the prompt style. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        string lyrics = string.Empty;
        bool isSongLyricsItemInDb = false;
        try
        {
            _logger.LogInformation("Attempting to get song lyrics for '{SongName}' by '{ArtistName}' from the database.", songName, artistName);
            SongLyricsItem dbResponse = await _cosmosDbService.GetSongLyricsItemAsync(artistName, songName, activity?.Id);

            lyrics = dbResponse.Lyrics;

            isSongLyricsItemInDb = true;
        }
        catch (NullReferenceException)
        {
            _logger.LogInformation("Song lyrics for '{SongName}' by '{ArtistName}' not found in database. Will attempt to get them from the internet.", songName, artistName);
            isSongLyricsItemInDb = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song lyrics item from Cosmos DB.");
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the song lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (!isSongLyricsItemInDb)
        {
            _logger.LogInformation("Searching for '{SongName}' by '{ArtistName}' on Genius.", songName, artistName);
            GeniusApiResponse<GeniusSearchResult>? geniusSearchResult = await _geniusApiService.SearchAsync(artistName, songName, activity?.Id);

            if (geniusSearchResult is null || geniusSearchResult.Response is null || geniusSearchResult.Response.Hits is null || geniusSearchResult.Response.Hits.Length == 0)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while searching for the song. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: isPrivateResponse
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            GeniusSearchResultHitItem? songResultItem = geniusSearchResult.Response.Hits.FirstOrDefault(item => item.Type == "song" && item.Result is not null && item.Result.LyricsState == "complete");

            if (songResultItem is null)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No results were found.").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: isPrivateResponse
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            _logger.LogInformation("Getting lyrics for '{SongName}' by '{ArtistName}' from Genius.", songName, artistName);
            try
            {
                lyrics = await _geniusApiService.GetLyricsAsync(songResultItem.Result!.Url!, activity?.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lyrics for song '{songId}'.", songName);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while getting the lyrics. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: isPrivateResponse
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            _logger.LogInformation("Adding lyrics for '{SongName}' by '{ArtistName}' to database.", songName, artistName);
            await _cosmosDbService.AddOrUpdateSongLyricsItemAsync(new(artistName, songName, lyrics), activity?.Id);
        }

        _logger.LogInformation("Analyzing lyrics for '{SongName}' by '{ArtistName}' with OpenAI.", songName, artistName);
        OpenAiChatCompletion? openAiChatCompletion;
        try
        {
            openAiChatCompletion = await _openAiService.GetLyricAnalysisAsync(
                artistName: artistName,
                songName: songName,
                lyrics: lyrics,
                promptStyle: promptStyle,
                parentActivityId: activity?.Id
            );

            if (openAiChatCompletion is null || openAiChatCompletion.Choices is null || openAiChatCompletion.Choices.Length == 0)
            {
                throw new NullReferenceException("The response from the OpenAI API was null.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyric analysis for song '{songId}'.", songName);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while analyzing the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: isPrivateResponse
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        StringBuilder lyricsResponseBuilder = new($"# \"{songName}\" by _{artistName}_\n\n");

        lyricsResponseBuilder.AppendLine($"## {promptStyle.AnalysisType}\n\n");

        string[] analysisLines = openAiChatCompletion.Choices[0].Message.Content.Split(Environment.NewLine);

        for (int i = 0; i < analysisLines.Length; i++)
        {
            if (i == analysisLines.Length - 1 && string.IsNullOrEmpty(analysisLines[i]))
            {
                break;
            }

            lyricsResponseBuilder.AppendLine($"> {analysisLines[i]}");
        }

        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ Note")
            .WithDescription(promptStyle.NoticeText)
            .WithColor(Color.DarkTeal)
            .WithFooter("(Powered by OpenAI's GPT-4)");

        _logger.LogInformation("Sending response to Discord.");
        try
        {
            await FollowupAsync(
                text: lyricsResponseBuilder.ToString(),
                embed: embedBuilder.Build(),
                ephemeral: isPrivateResponse
            );

            if (lyricsAnalyzerConfig.RateLimitEnabled && !userIsOnRateLimitIgnoreList)
            {
                _logger.LogInformation("Updating rate limit for user '{UserId}' in database.", Context.User.Id);
                lyricsAnalyzerUserRateLimit!.IncrementRequestCount();
                lyricsAnalyzerUserRateLimit!.LastRequestTime = DateTimeOffset.UtcNow;

                await _cosmosDbService.AddOrUpdateLyricsAnalyzerUserRateLimitAsync(lyricsAnalyzerUserRateLimit, activity?.Id);
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