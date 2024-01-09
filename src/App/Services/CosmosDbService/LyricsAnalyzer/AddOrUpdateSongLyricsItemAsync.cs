using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    public async Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem) => await AddOrUpdateSongLyricsItemAsync(songLyricsItem, null);
    public async Task AddOrUpdateSongLyricsItemAsync(SongLyricsItem songLyricsItem, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbAddOrUpdateSongLyricsItemActivity(songLyricsItem, parentActivityId);

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
            _logger.LogInformation("Adding item for '{Id}' to the 'song-lyrics' container.", songLyricsItem.Id);
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(songLyricsItem.PartitionKey)
            );
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            _logger.LogInformation(" The item for '{Id}' already exists. Replacing item for '{Id}' in the 'song-lyrics' container.", songLyricsItem.Id, songLyricsItem.Id);
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: songLyricsItem.Id,
                partitionKey: new PartitionKey(songLyricsItem.PartitionKey)
            );
        }
    }
}