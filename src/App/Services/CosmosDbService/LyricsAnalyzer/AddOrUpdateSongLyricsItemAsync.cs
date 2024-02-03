using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Logging.CosmosDb;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    /// <summary>
    /// Adds or updates the song lyrics item in the database.
    /// </summary>
    /// <param name="songLyricsItem">The song lyrics.</param>
    /// <returns></returns>
    public async Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem) => await AddOrUpdateSongLyricsItemAsync(songLyricsItem, null);

    /// <summary>
    /// Adds or updates the song lyrics item in the database.
    /// </summary>
    /// <param name="songLyricsItem">The song lyrics.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns></returns>
    public async Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbAddOrUpdateSongLyricsItemActivity(songLyricsItem, parentActivityId);

        _logger.LogAddOrUpdateOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerSongLyrics,
            id: songLyricsItem.Id
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "song-lyrics"
        );

        using MemoryStream itemPayload = new();
        await JsonSerializer.SerializeAsync(
            utf8Json: itemPayload,
            value: songLyricsItem,
            jsonTypeInfo: DatabaseJsonContext.Default.SongLyricsItem
        );

        try
        {
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(songLyricsItem.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerSongLyrics,
                id: songLyricsItem.Id,
                state: CosmosDbServiceLoggingConstants.OperationTypes.Added
            );
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: songLyricsItem.Id,
                partitionKey: new PartitionKey(songLyricsItem.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerSongLyrics,
                id: songLyricsItem.Id,
                state: CosmosDbServiceLoggingConstants.OperationTypes.Updated
            );
        }
    }
}