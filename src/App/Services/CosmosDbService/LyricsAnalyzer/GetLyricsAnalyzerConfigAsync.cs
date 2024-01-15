using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Logging.CosmosDb;
using MuzakBot.App.Models.CosmosDb;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    /// <summary>
    /// Gets the lyrics analyzer config from the database.
    /// </summary>
    /// <remarks>
    /// If the config does not exist, a new one will be created.
    /// </remarks>
    /// <returns>The retrieved lyrics analyzer config.</returns>
    public async Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync() => await GetLyricsAnalyzerConfigAsync(null);

    /// <summary>
    /// Gets the lyrics analyzer config from the database.
    /// </summary>
    /// <remarks>
    /// If the config does not exist, a new one will be created.
    /// </remarks>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The retrieved lyrics analyzer config.</returns>
    public async Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync(string? parentActivityId)
    {
        using var activity = _activitySource.StartDbGetLyricsAnalyzerConfigActivity(
            parentActivityId: parentActivityId
        );

        _logger.LogGetOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
            id: "default"
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "command-configs"
        );

        var query = new QueryDefinition(
            query: "SELECT * FROM c WHERE c.partitionKey = 'lyricsanalyzer-config'"
        );

        CosmosDbResponse<LyricsAnalyzerConfig>? cosmosDbResponse;

        try
        {
            using FeedIterator feedIterator = container.GetItemQueryStreamIterator(
                queryDefinition: query,
                requestOptions: new()
                {
                    PartitionKey = new PartitionKey("lyricsanalyzer-config")
                }
            );

            using ResponseMessage response = await feedIterator.ReadNextAsync();

            cosmosDbResponse = await JsonSerializer.DeserializeAsync(
                utf8Json: response.Content,
                jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseLyricsAnalyzerConfig
            );
        }
        catch (Exception ex)
        {
            _logger.LogGetOperationFailed(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
                id: "default",
                exception: ex
            );
            activity?.SetStatus(ActivityStatusCode.Error);

            throw;
        }

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null || cosmosDbResponse.Documents.Length == 0)
        {
            _logger.LogGetOperationNotFound(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
                id: "default"
            );

            LyricsAnalyzerConfig lyricsAnalyzerConfig = new(true);

            try
            {
                await AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig);
            }
            catch (Exception ex)
            {
                _logger.LogGetOperationFailed(
                    itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
                    id: "default",
                    exception: ex
                );

                activity?.SetStatus(ActivityStatusCode.Error);
                
                throw;
            }

            _logger.LogGetOperationFound(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
                id: "default"
            );

            return lyricsAnalyzerConfig;
        }

        _logger.LogGetOperationFound(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerConfig,
            id: "default"
        );

        return cosmosDbResponse.Documents.FirstOrDefault()!;
    }
}