using System.Diagnostics;
using System.Text;

using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
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

        LyricsAnalyzerPromptStyle promptStyle;
        try
        {
            promptStyle = await GetPromptStyleAsync(promptMode, activity?.Id);
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

        _logger.LogInformation("Analyzing lyrics for '{SongName}' by '{ArtistName}' with OpenAI.", songName, artistName);
        OpenAiChatCompletion openAiChatCompletion;
        try
        {
            openAiChatCompletion = await RunLyricsAnalysisAsync(artistName, songName, lyrics, promptStyle, activity?.Id);
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