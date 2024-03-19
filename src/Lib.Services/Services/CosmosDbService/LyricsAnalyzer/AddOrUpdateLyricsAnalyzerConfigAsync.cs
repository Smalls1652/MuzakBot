using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Services.Extensions.Telemetry;
using MuzakBot.Lib.Services.Logging.CosmosDb;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Lib.Services;

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

        _logger.LogAddOrUpdateOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
            id: lyricsAnalyzerConfig.Id
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "command-configs"
        );

        using MemoryStream itemPayload = new();
        await JsonSerializer.SerializeAsync(
            utf8Json: itemPayload,
            value: lyricsAnalyzerConfig,
            jsonTypeInfo: DatabaseJsonContext.Default.LyricsAnalyzerConfig
        );

        try
        {
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(lyricsAnalyzerConfig.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
                id: lyricsAnalyzerConfig.Id,
                state: CosmosDbServiceLoggingConstants.OperationTypes.Added
            );
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: lyricsAnalyzerConfig.Id,
                partitionKey: new PartitionKey(lyricsAnalyzerConfig.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
                id: lyricsAnalyzerConfig.Id,
                state: CosmosDbServiceLoggingConstants.OperationTypes.Updated
            );
        }
    }
}