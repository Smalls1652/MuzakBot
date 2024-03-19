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
    /// Gets a specific lyrics analyzer prompt style from the database.
    /// </summary>
    /// <param name="shortName">The short name of the prompt style.</param>
    /// <returns>The retrieved lyrics analyzer prompt style.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<LyricsAnalyzerPromptStyle?> GetLyricsAnalyzerPromptStyleAsync(string shortName) => await GetLyricsAnalyzerPromptStyleAsync(shortName, null);

    /// <summary>
    /// Gets a specific lyrics analyzer prompt style from the database.
    /// </summary>
    /// <param name="shortName">The short name of the prompt style.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The retrieved lyrics analyzer prompt style.</returns>
    /// <exception cref="Exception"></exception>
    public async Task<LyricsAnalyzerPromptStyle?> GetLyricsAnalyzerPromptStyleAsync(string shortName, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbGetLyricsAnalyzerPromptStyleActivity(
            shortName: shortName,
            parentActivityId: parentActivityId
        );

        _logger.LogGetOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
            id: shortName
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "prompt-styles"
        );

        var query = new QueryDefinition(
            query: "SELECT * FROM c WHERE c.partitionKey = 'prompt-style' AND c.shortName = @shortName"
        )
            .WithParameter("@shortName", shortName);

        CosmosDbResponse<LyricsAnalyzerPromptStyle>? cosmosDbResponse;

        try
        {
            using FeedIterator feedIterator = container.GetItemQueryStreamIterator(
                queryDefinition: query,
                requestOptions: new()
                {
                    PartitionKey = new PartitionKey("prompt-style")
                }
            );

            using ResponseMessage response = await feedIterator.ReadNextAsync();

            cosmosDbResponse = await JsonSerializer.DeserializeAsync(
                utf8Json: response.Content,
                jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseLyricsAnalyzerPromptStyle
            );
        }
        catch (Exception ex)
        {
            _logger.LogGetOperationFailed(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
                id: shortName,
                exception: ex
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            throw;
        }

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null)
        {
            return null;
        }

        return cosmosDbResponse.Documents.FirstOrDefault();
    }
}