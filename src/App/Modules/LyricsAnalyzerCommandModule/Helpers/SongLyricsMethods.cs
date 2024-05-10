using System.Text;

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
    private async Task<string> GetSongLyricsAsync(string artistName, string songName, string? parentActivityId)
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

        GeniusSearchResultHitItem? songResultItem = geniusSearchResult.Response.Hits.FirstOrDefault(item => item.Type == "song" && item.Result is not null && item.Result.LyricsState == "complete");

        if (songResultItem is null)
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
            GeniusUrl = songResultItem.Result!.Url!
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

            lyrics = await RunSongLyricsRequestFallbackJobAsync(songResultItem, artistName, songName, parentActivityId);
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
            while (!isSongLyricsJobFinished)
            {
                using (var songLyricsDbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext())
                {
                    SongLyricsRequestJob songLyricsRequestJobStatus = await songLyricsDbContext.SongLyricsRequestJobs.FirstAsync(item => item.Id == songLyricsRequestJob.Id);

                    // If the standalone service has not acknowledged the request, throw an exception.
                    if (!songLyricsRequestJobStatus.StandaloneServiceAcknowledged)
                    {
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

                    // If the standalone service indicates that the fallback method is needed, throw an exception.
                    if (songLyricsRequestJobStatus.FallbackMethodNeeded)
                    {
                        _logger.LogWarning("Fallback method is needed. Continuing with the fallback method instead.");

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
    /// <param name="songResultItem">The Genius search result item for the song.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity.</param>
    /// <returns>The lyrics of the specified song.</returns>
    /// <exception cref="SongRequestJobException">Thrown when an error occurs while attempting to get song lyrics using the fallback method.</exception>
    private async Task<string?> RunSongLyricsRequestFallbackJobAsync(GeniusSearchResultHitItem songResultItem, string artistName, string songName, string? parentActivityId)
    {
        string? lyrics = null;

        try
        {
            lyrics = await _geniusApiService.GetLyricsAsync(songResultItem.Result!.Url!, parentActivityId);
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
}