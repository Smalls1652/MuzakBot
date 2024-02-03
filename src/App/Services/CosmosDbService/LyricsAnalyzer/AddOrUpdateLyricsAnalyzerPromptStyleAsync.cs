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
    /// Adds or updates the lyrics analyzer prompt style in the database.
    /// </summary>
    /// <param name="promptStyle">The prompt style to add or update.</param>
    /// <returns></returns>
    public async Task AddOrUpdateLyricsAnalyzerPromptStyleAsync(LyricsAnalyzerPromptStyle promptStyle) => await AddOrUpdateLyricsAnalyzerPromptStyleAsync(promptStyle, null);

    /// <summary>
    /// Adds or updates the lyrics analyzer prompt style in the database.
    /// </summary>
    /// <param name="promptStyle">The prompt style to add or update.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns></returns>
    public async Task AddOrUpdateLyricsAnalyzerPromptStyleAsync(LyricsAnalyzerPromptStyle promptStyle, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbAddOrUpdateLyricsAnalyzerPromptStyleActivity(
            promptStyle: promptStyle,
            parentActivityId: parentActivityId
        );

        _logger.LogAddOrUpdateOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
            id: promptStyle.Id
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "prompt-styles"
        );

        string promptStyleJson = JsonSerializer.Serialize(
            value: promptStyle,
            jsonTypeInfo: DatabaseJsonContext.Default.LyricsAnalyzerPromptStyle
        );

        using MemoryStream itemPayload = new();
        await JsonSerializer.SerializeAsync(
            utf8Json: itemPayload,
            value: promptStyle,
            jsonTypeInfo: DatabaseJsonContext.Default.LyricsAnalyzerPromptStyle
        );

        try
        {
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(promptStyle.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
                id: promptStyle.Id,
                state: CosmosDbServiceLoggingConstants.OperationTypes.Added
            );
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: promptStyle.Id,
                partitionKey: new PartitionKey(promptStyle.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerPromptStyle,
                id: promptStyle.Id,
                state: CosmosDbServiceLoggingConstants.OperationTypes.Updated
            );
        }
    }
}
