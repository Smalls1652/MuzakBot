using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    /// <summary>
    /// Adds or updates the lyrics analyzer config in the database.
    /// </summary>
    /// <param name="lyricsAnalyzerConfig">The config to add or update.</param>
    /// <returns></returns>
    public async Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig) => await AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig, null);

    /// <summary>
    /// Adds or updates the lyrics analyzer config in the database.
    /// </summary>
    /// <param name="lyricsAnalyzerConfig">The config to add or update.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns></returns>
    public async Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbGetLyricsAnalyzerConfigActivity(
            parentActivityId: parentActivityId
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "command-configs"
        );

        _logger.LogInformation("Serializing lyrics analyzer config.");
        using MemoryStream itemPayload = new();
        await JsonSerializer.SerializeAsync(
            utf8Json: itemPayload,
            value: lyricsAnalyzerConfig,
            jsonTypeInfo: DatabaseJsonContext.Default.LyricsAnalyzerConfig
        );

        _logger.LogInformation("Pushing to the database.");

        try
        {
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(lyricsAnalyzerConfig.PartitionKey)
            );

            _logger.LogInformation("Lyrics analyzer config added.");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: lyricsAnalyzerConfig.Id,
                partitionKey: new PartitionKey(lyricsAnalyzerConfig.PartitionKey)
            );

            _logger.LogInformation("Lyrics analyzer config updated.");
        }
    }
}