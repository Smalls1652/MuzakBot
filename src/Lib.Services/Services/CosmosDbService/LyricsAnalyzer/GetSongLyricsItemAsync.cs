using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Services.Extensions.Telemetry;
using MuzakBot.Lib.Services.Logging.CosmosDb;
using MuzakBot.Lib.Models.CosmosDb;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Lib.Services;

public partial class CosmosDbService
{
    /// <summary>
    /// Gets a song lyrics item from the database.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The retrieved song lyrics item.</returns>
    public async Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName) => await GetSongLyricsItemAsync(artistName, songName, null);

    /// <summary>
    /// Gets a song lyrics item from the database.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The retrieved song lyrics item.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<SongLyricsItem> GetSongLyricsItemAsync(string artistName, string songName, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbGetSongLyricsItemActivity(artistName, songName, parentActivityId);

        _logger.LogGetOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerSongLyrics,
            id: $"{artistName} - {songName}"
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "song-lyrics"
        );

        var query = new QueryDefinition(
            query: "SELECT * FROM c WHERE c.partitionKey = 'song-lyrics-item' AND c.artistName = @artistName AND c.songName = @songName"
        )
        .WithParameter(
            name: "@artistName",
            value: artistName
        )
        .WithParameter(
            name: "@songName",
            value: songName
        );

        using FeedIterator feedIterator = container.GetItemQueryStreamIterator(
            queryDefinition: query,
            requestOptions: new()
            {
                PartitionKey = new PartitionKey("song-lyrics-item")
            }
        );

        using ResponseMessage response = await feedIterator.ReadNextAsync();

        CosmosDbResponse<SongLyricsItem>? cosmosDbResponse = await JsonSerializer.DeserializeAsync(
            utf8Json: response.Content,
            jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseSongLyricsItem
        );

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null || cosmosDbResponse.Documents.Length == 0)
        {
            _logger.LogGetOperationNotFound(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerSongLyrics,
                id: $"{artistName} - {songName}"
            );

            activity?.SetStatus(ActivityStatusCode.Error);
            throw new NullReferenceException("The Cosmos DB response was null.");
        }

        _logger.LogGetOperationFound(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerSongLyrics,
            id: $"{artistName} - {songName}"
        );

        return cosmosDbResponse.Documents.FirstOrDefault()!;
    }
}