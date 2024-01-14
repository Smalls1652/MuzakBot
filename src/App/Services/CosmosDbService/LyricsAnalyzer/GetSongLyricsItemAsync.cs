using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.CosmosDb;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

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

        _logger.LogInformation("Getting item for '{ArtistName}' - '{SongName}' from the 'song-lyrics' container.", artistName, songName);
        using ResponseMessage response = await feedIterator.ReadNextAsync();

        CosmosDbResponse<SongLyricsItem>? cosmosDbResponse = await JsonSerializer.DeserializeAsync(
            utf8Json: response.Content,
            jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseSongLyricsItem
        );

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null || cosmosDbResponse.Documents.Length == 0)
        {
            _logger.LogWarning("An item for '{ArtistName}' - '{SongName}' was not found in the 'song-lyrics' container.", artistName, songName);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw new NullReferenceException("The Cosmos DB response was null.");
        }

        return cosmosDbResponse.Documents.FirstOrDefault()!;
    }
}