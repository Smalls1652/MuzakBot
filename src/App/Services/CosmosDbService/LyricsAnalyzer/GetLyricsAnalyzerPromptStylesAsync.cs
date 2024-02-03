using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Logging.CosmosDb;
using MuzakBot.App.Models.CosmosDb;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    /// <summary>
    /// Gets all lyrics analyzer prompt styles from the database.
    /// </summary>
    /// <returns>The retrieved lyrics analyzer prompt styles.</returns>
    public async Task<LyricsAnalyzerPromptStyle[]> GetLyricsAnalyzerPromptStylesAsync() => await GetLyricsAnalyzerPromptStylesAsync(null);

    /// <summary>
    /// Gets all lyrics analyzer prompt styles from the database.
    /// </summary>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The retrieved lyrics analyzer prompt styles.</returns>
    public async Task<LyricsAnalyzerPromptStyle[]> GetLyricsAnalyzerPromptStylesAsync(string? parentActivityId)
    {
        using var activity = _activitySource.StartDbGetLyricsAnalyzerPromptStylesActivity(
            parentActivityId: parentActivityId
        );

        _logger.LogGetOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
            id: "all"
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "prompt-styles"
        );

        var query = new QueryDefinition(
            query: "SELECT * FROM c WHERE c.partitionKey = 'prompt-style'"
        );

        int resultCount = await GetResultCountAsync(
            container: container,
            coreQuery: "WHERE c.partitionKey = 'prompt-style'"
        );

        _logger.LogInformation("Found {Count} prompt styles in the database.", resultCount);

        LyricsAnalyzerPromptStyle[] promptStyles = new LyricsAnalyzerPromptStyle[resultCount];

        using FeedIterator feedIterator = container.GetItemQueryStreamIterator(
            queryDefinition: query,
            requestOptions: new()
            {
                PartitionKey = new PartitionKey("prompt-style")
            }
        );

        
        while (feedIterator.HasMoreResults)
        {
            using ResponseMessage response = await feedIterator.ReadNextAsync();

            CosmosDbResponse<LyricsAnalyzerPromptStyle>? cosmosDbResponse = await JsonSerializer.DeserializeAsync(
                utf8Json: response.Content,
                jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseLyricsAnalyzerPromptStyle
            );

            if (cosmosDbResponse is null || cosmosDbResponse.Documents is null)
            {
                NullReferenceException nullResponseException = new(
                    message: "The Cosmos DB response or its documents were null."
                );

                _logger.LogGetOperationFailed(
                    itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
                    id: "all",
                    exception: nullResponseException
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                throw nullResponseException;
            }

            int i = 0;
            foreach (var promptStyleItem in cosmosDbResponse!.Documents!)
            {
                promptStyles[i] = promptStyleItem;
                i++;
            }
        }

        _logger.LogGetOperationFound(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
            id: "all"
        );

        return promptStyles;
    }
}