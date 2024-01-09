using System.Diagnostics;
using System.Text;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Handlers;
using MuzakBot.App.Models.Genius;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Models.OpenAi;
using MuzakBot.App.Services;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Handles the 'lyricsanalyzer' slash command.
    /// </summary>
    /// <param name="artistId">The ID of the artist.</param>
    /// <param name="songId">The ID of the song.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    [SlashCommand(
        name: "lyricsanalyzer",
        description: "Get an analysis of the lyrics of a song"
    )]
    private async Task HandleLyricsAnalyzerAsync(
        [Summary("artistName", "The name of an artist"),
         Autocomplete(typeof(ArtistSearchAutoCompleteHandler))
        ]
        string artistId,
        [Summary(name: "songName", description: "The name of a song"),
         Autocomplete(typeof(ArtistSongSearchAutoCompleteHandler))
        ]
        string songId
    )
    {
        using var activity = _activitySource.StartHandleGetLyricsAsyncActivity(artistId, songId, Context);

        await DeferAsync(
                ephemeral: false
            );

        _logger.LogInformation("Looking up artist '{artistId}' and song '{songId}'.", artistId, songId);
        MusicBrainzArtistItem? artistItem = null;
        try
        {
            artistItem = await _musicBrainzService.LookupArtistAsync(artistId, activity?.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up artist '{artistId}'.", artistId);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while looking up the artist. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        MusicBrainzRecordingItem? recordingItem = null;

        try
        {
            recordingItem = await _musicBrainzService.LookupRecordingAsync(songId, activity?.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error looking up song '{songId}'.", songId);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while looking up the song. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        if (artistItem is null || recordingItem is null)
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("No results found for that artist and song. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
        
        _logger.LogInformation("Searching for '{SongName}' by '{ArtistName}' on Genius.", recordingItem.Title, artistItem.Name);
        GeniusApiResponse<GeniusSearchResult>? geniusSearchResult = await _geniusApiService.SearchAsync(artistItem.Name, recordingItem.Title, activity?.Id);

        if (geniusSearchResult is null || geniusSearchResult.Response is null || geniusSearchResult.Response.Hits is null || geniusSearchResult.Response.Hits.Length == 0)
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while searching for the song. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
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
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        _logger.LogInformation("Getting lyrics for '{SongName}' by '{ArtistName}' from Genius.", recordingItem.Title, artistItem.Name);
        string lyrics = string.Empty;
        try
        {
            lyrics = await _geniusApiService.GetLyricsAsync(songResultItem.Result!.Path!, activity?.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyrics for song '{songId}'.", songId);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        _logger.LogInformation("Analyzing lyrics for '{SongName}' by '{ArtistName}' with OpenAI.", recordingItem.Title, artistItem.Name);
        OpenAiChatCompletion? openAiChatCompletion;
        try
        {
            openAiChatCompletion = await _openAiService.GetLyricAnalysisAsync(
                artistName: artistItem.Name,
                songName: recordingItem.Title,
                lyrics: lyrics,
                parentActivityId: activity?.Id
            );

            if (openAiChatCompletion is null || openAiChatCompletion.Choices is null || openAiChatCompletion.Choices.Length == 0)
            {
                throw new NullReferenceException("The response from the OpenAI API was null.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lyric analysis for song '{songId}'.", songId);
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while analyzing the lyrics. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        StringBuilder lyricsResponseBuilder = new();

        string[] analysisLines = openAiChatCompletion.Choices[0].Message.Content.Split(Environment.NewLine);

        for (int i = 0; i < analysisLines.Length; i++)
        {
            if (i == analysisLines.Length - 1 && string.IsNullOrEmpty(analysisLines[i]))
            {
                break;
            }

            lyricsResponseBuilder.AppendLine($"> {analysisLines[i]}");
        }

        _logger.LogInformation("Sending response to Discord.");
        try
        {
            await FollowupAsync(
                text: lyricsResponseBuilder.ToString(),
                ephemeral: false
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending response to Discord.");
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while sending the response. 😥").Build(),
                components: GenerateRemoveComponent().Build(),
                ephemeral: false
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }
    }
}