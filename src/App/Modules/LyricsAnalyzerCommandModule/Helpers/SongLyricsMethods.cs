using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MuzakBot.Lib;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.Genius;
using MuzakBot.Lib.Models.QueueMessages;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Gets the lyrics for a specified song.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The lyrics of the specified song.</returns>
    /// <exception cref="LyricsAnalyzerDbException">Thrown when an error occurs while attempting to get song lyrics from the database.</exception>
    /// <exception cref="GeniusApiException">Thrown when an error occurs while attempting to get song lyrics from the Genius API.</exception>
    /// <exception cref="SongRequestJobException">Thrown when an error occurs while attempting to get song lyrics using the standalone service or the fallback method.</exception>
    private async Task<string> GetSongLyricsAsync(string artistName, string songName, string appleMusicId, string? parentActivityId)
    {
        string? lyrics = null;
        bool isSongLyricsItemInDb = false;

        // Attempt to get song lyrics from the database.
        try
        {
            _logger.LogInformation("Attempting to get song lyrics for '{SongName}' by '{ArtistName}' from the database.", songName, artistName);

            using (var songLyricsDbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext())
            {
                SongLyricsItem dbResponse = await songLyricsDbContext.SongLyricsItems
                    .FirstOrDefaultAsync(item => item.ArtistName == artistName && item.SongName == songName)
                    ?? throw new NullReferenceException("Song lyrics item not found in database.");

                lyrics = dbResponse.Lyrics;

                isSongLyricsItemInDb = true;
            }
        }
        catch (NullReferenceException)
        {
            _logger.LogInformation("Song lyrics for '{SongName}' by '{ArtistName}' not found in database. Will attempt to get them from the internet.", songName, artistName);
            isSongLyricsItemInDb = false;
        }
        catch (Exception ex)
        {
            throw new LyricsAnalyzerDbException(
                message: "An error occurred while attempting to get song lyrics from the database.",
                innerException: ex
            );
        }

        // If song lyrics were found in the database, return them.
        if (isSongLyricsItemInDb)
        {
            return lyrics!;
        }

        // Attempt to resolve a Genius URL for the specified song.
        _logger.LogInformation("Searching for '{SongName}' by '{ArtistName}' on Genius.", songName, artistName);
        GeniusApiResponse<GeniusSearchResult>? geniusSearchResult = await _geniusApiService.SearchAsync(artistName, songName, parentActivityId);

        if (geniusSearchResult is null || geniusSearchResult.Response is null || geniusSearchResult.Response.Hits is null || geniusSearchResult.Response.Hits.Length == 0)
        {
            throw new GeniusApiException(
                exceptionType: GeniusApiExceptionType.NoResults,
                message: "No results found for the specified song."
            );
        }

        GeniusSearchResultHitItem[] songResultItems = Array.FindAll(
            array: geniusSearchResult.Response.Hits,
            match: item => item.Type == "song" && item.Result is not null && item.Result.LyricsState == "complete"
        );

        if (songResultItems.Length == 0)
        {
            throw new GeniusApiException(
                exceptionType: GeniusApiExceptionType.NoSongsFound,
                message: "No complete songs found for the requested artist and song."
            );
        }

        string? songUrl = null;

        foreach (var searchHit in songResultItems)
        {
            GeniusApiResponse<GeniusSongResult>? songLookup = await _geniusApiService.GetSongAsync(searchHit.Result!.Id, parentActivityId);

            if (songLookup is not null && songLookup.Response is not null && songLookup.Response.Song is not null)
            {
                if (songLookup.Response.Song.AppleMusicId == appleMusicId)
                {
                    songUrl = songLookup.Response.Song.Url;
                    break;
                }
            }
        }

        if (songUrl is null)
        {
            _logger.LogWarning("Apple Music ID not found in Genius search results. Attempting to find song by title and artist name.");

            string? coreSongName = null;

            if (TrimmedSongNameRegex().IsMatch(songName))
            {
                Match match = TrimmedSongNameRegex().Match(songName);
                coreSongName = match.Groups["songName"].Value;
            }

            GeniusSearchResultHitItem? firstUsableSongItem = Array.Find(
                array: songResultItems,
                match: item =>
                    item.Result is not null
                    && (
                            item.Result.Title == songName
                            && item.Result.PrimaryArtist.Name == artistName
                        || 
                            coreSongName is not null
                            && item.Result.Title is not null
                            && PunctuationRegex().Replace(item.Result.Title, "") == PunctuationRegex().Replace(coreSongName, "")
                            && item.Result.PrimaryArtist.Name == artistName
                    )
            );

            songUrl = firstUsableSongItem?.Result?.Url;
        }

        if (songUrl is null)
        {
            throw new GeniusApiException(
                exceptionType: GeniusApiExceptionType.NoSongsFound,
                message: "No songs found for the requested artist and song."
            );
        }

        // Start the process of scraping the lyrics from the Genius page.
        _logger.LogInformation("Getting lyrics for '{SongName}' by '{ArtistName}' from Genius.", songName, artistName);
        SongLyricsRequestMessage songLyricsRequestMessage = new()
        {
            JobId = Guid.NewGuid().ToString(),
            ArtistName = artistName,
            SongTitle = songName,
            GeniusUrl = songUrl
        };

        try
        {
            // Attempt to get song lyrics using the standalone service.
            lyrics = await RunSongLyricsRequestJobAsync(songLyricsRequestMessage);
        }
        catch (SongRequestJobException ex) when (ex.ExceptionType == SongRequestJobExceptionType.FallbackMethodNeeded || ex.ExceptionType == SongRequestJobExceptionType.StandaloneServiceNotAcknowledged || ex.ExceptionType == SongRequestJobExceptionType.LyricsReturnedNull)
        {
            // Attempt to get song lyrics using the fallback method, when the standalone service fails.
            _logger.LogWarning(ex, "An error occurred while attempting to get song lyrics using the standalone service. Attempting to get lyrics using the fallback method.");

            lyrics = await RunSongLyricsRequestFallbackJobAsync(songUrl, artistName, songName, parentActivityId);
        }
        catch (Exception)
        {
            throw;
        }

        if (lyrics is null || string.IsNullOrEmpty(lyrics))
        {
            throw new SongRequestJobException(
                exceptionType: SongRequestJobExceptionType.LyricsReturnedNull,
                message: "Returned lyrics were null or empty."
            );
        }

        return lyrics;
    }

    /// <summary>
    /// Runs a song lyrics request job using the standalone service.
    /// </summary>
    /// <param name="requestMessage">The message for the song lyrics request job.</param>
    /// <returns>The lyrics of the specified song.</returns>
    /// <exception cref="SongRequestJobException">Thrown when an error occurs while attempting to get song lyrics using the standalone service.</exception>
    private async Task<string?> RunSongLyricsRequestJobAsync(SongLyricsRequestMessage requestMessage)
    {
        string? lyrics = null;

        // Create a new song lyrics request job and add it to the database.
        SongLyricsRequestJob songLyricsRequestJob;
        using (var songLyricsDbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext())
        {
            songLyricsRequestJob = new(requestMessage);

            await songLyricsDbContext.SongLyricsRequestJobs.AddAsync(songLyricsRequestJob);
            await songLyricsDbContext.SaveChangesAsync();
        }

        try
        {
            // Send the song lyrics request message to the queue.
            string songLyricsRequestJson = JsonSerializer.Serialize(
                value: requestMessage,
                jsonTypeInfo: QueueMessageJsonContext.Default.SongLyricsRequestMessage
            );
            string songLyricsRequestJsonBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(songLyricsRequestJson));
            var requestJobQueueResponse = await _queueClientService.QueueClient.SendMessageAsync(songLyricsRequestJsonBase64);

            // Wait for the standalone service to acknowledge the request
            // and process the job.
            await Task.Delay(5000);
            bool isSongLyricsJobFinished = false;
            Stopwatch acknowledgedStopwatch = Stopwatch.StartNew();
            while (!isSongLyricsJobFinished)
            {
                using (var songLyricsDbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext())
                {
                    SongLyricsRequestJob songLyricsRequestJobStatus = await songLyricsDbContext.SongLyricsRequestJobs.FirstAsync(item => item.Id == songLyricsRequestJob.Id);

                    // If the standalone service has not acknowledged the request, throw an exception.
                    if (!songLyricsRequestJobStatus.StandaloneServiceAcknowledged)
                    {
                        if (acknowledgedStopwatch.Elapsed.TotalMinutes < 2)
                        {
                            continue;
                        }

                        acknowledgedStopwatch.Stop();

                        _logger.LogWarning("Standalone service has not acknowledged the request. Continuing with the fallback method instead.");

                        await _queueClientService.QueueClient.DeleteMessageAsync(
                            messageId: requestJobQueueResponse.Value.MessageId,
                            popReceipt: requestJobQueueResponse.Value.PopReceipt
                        );

                        throw new SongRequestJobException(
                            exceptionType: SongRequestJobExceptionType.StandaloneServiceNotAcknowledged,
                            message: "Standalone service has not acknowledged the request. Fallback method is needed."
                        );
                    }
                    else
                    {
                        acknowledgedStopwatch.Stop();
                    }

                    // If the standalone service indicates that the fallback method is needed, throw an exception.
                    if (songLyricsRequestJobStatus.FallbackMethodNeeded)
                    {
                        _logger.LogWarning("Fallback method is needed. Continuing with the fallback method instead.");

                        acknowledgedStopwatch.Stop();

                        throw new SongRequestJobException(
                            exceptionType: SongRequestJobExceptionType.FallbackMethodNeeded,
                            message: "Standalone service has indicated that the fallback method is needed."
                        );
                    }

                    if (songLyricsRequestJobStatus.IsCompleted)
                    {
                        SongLyricsItem songLyricsItemByJob = await songLyricsDbContext.SongLyricsItems.FirstAsync(item => item.Id == songLyricsRequestJobStatus.SongLyricsItemId!);

                        lyrics = songLyricsItemByJob.Lyrics;
                        isSongLyricsJobFinished = true;
                    }
                }
            }

            // If the lyrics are null or empty, throw an exception.
            if (lyrics is null || string.IsNullOrEmpty(lyrics))
            {
                throw new SongRequestJobException(
                    exceptionType: SongRequestJobExceptionType.LyricsReturnedNull,
                    message: "Lyrics returned from the standalone service were null or empty."
                );
            }
        }
        finally
        {
            // Remove the song lyrics request job from the database.
            using (var songLyricsDbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext())
            {
                songLyricsDbContext.SongLyricsRequestJobs.Remove(songLyricsRequestJob);
                await songLyricsDbContext.SaveChangesAsync();
            }
        }

        return lyrics;
    }

    /// <summary>
    /// Runs a song lyrics request job using the fallback method.
    /// </summary>
    /// <param name="url">The URL to the Genius lyrics page.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The lyrics of the specified song.</returns>
    /// <exception cref="SongRequestJobException">Thrown when an error occurs while attempting to get song lyrics using the fallback method.</exception>
    private async Task<string?> RunSongLyricsRequestFallbackJobAsync(string url, string artistName, string songName, string? parentActivityId)
    {
        string? lyrics = null;

        try
        {
            lyrics = await _geniusApiService.GetLyricsAsync(url, parentActivityId);
        }
        catch (Exception ex)
        {
            throw new SongRequestJobException(
                exceptionType: SongRequestJobExceptionType.FallbackJobFailed,
                message: "An error occurred while attempting to get song lyrics using the fallback method.",
                innerException: ex
            );
        }

        using (var songLyricsDbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext())
        {
            await songLyricsDbContext.SongLyricsItems.AddAsync(
                new(
                    artistName: artistName,
                    songName: songName,
                    lyrics: lyrics!
                )
            );

            _logger.LogInformation("Adding lyrics for '{SongName}' by '{ArtistName}' to database.", songName, artistName);
            await songLyricsDbContext.SaveChangesAsync();
        }

        return lyrics;
    }

    [GeneratedRegex(
        pattern: @"^(?'songName'.+?(\s\(Remix\)|))($|\s[\(\[].+[\)\]])"
    )]
    internal partial Regex TrimmedSongNameRegex();

    [GeneratedRegex(
        pattern: @"[^a-zA-Z0-9\x00-\x1F\x7F\s\t\n\r]"
    )]
    internal partial Regex PunctuationRegex();
}
