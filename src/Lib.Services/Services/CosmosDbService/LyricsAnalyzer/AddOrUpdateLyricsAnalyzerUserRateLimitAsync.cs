using System.Net;
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
    /// Adds or updates the lyrics analyzer rate limit for a user in the database.
    /// </summary>
    /// <param name="lyricsAnalyzerUserRateLimit">The lyrics analyzer rate limit for the user.</param>
    /// <returns></returns>
    public async Task AddOrUpdateLyricsAnalyzerUserRateLimitAsync(LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit) => await AddOrUpdateLyricsAnalyzerUserRateLimitAsync(lyricsAnalyzerUserRateLimit, null);

    /// <summary>
    /// Adds or updates the lyrics analyzer rate limit for a user in the database.
    /// </summary>
    /// <param name="lyricsAnalyzerUserRateLimit">The lyrics analyzer rate limit for the user.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns></returns>
    public async Task AddOrUpdateLyricsAnalyzerUserRateLimitAsync(LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbAddOrUpdateLyricsAnalyzerUserRateLimitActivity(
            lyricsAnalyzerUserRateLimit: lyricsAnalyzerUserRateLimit,
            parentActivityId: parentActivityId
        );

        _logger.LogAddOrUpdateOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
            id: lyricsAnalyzerUserRateLimit.UserId.ToString()
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "rate-limit"
        );

        using MemoryStream itemPayload = new();
        await JsonSerializer.SerializeAsync(
            utf8Json: itemPayload,
            value: lyricsAnalyzerUserRateLimit,
            jsonTypeInfo: DatabaseJsonContext.Default.LyricsAnalyzerUserRateLimit
        );

        try
        {
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(lyricsAnalyzerUserRateLimit.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
                id: lyricsAnalyzerUserRateLimit.UserId.ToString(),
                state: CosmosDbServiceLoggingConstants.OperationTypes.Added
            );
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: lyricsAnalyzerUserRateLimit.Id,
                partitionKey: new PartitionKey(lyricsAnalyzerUserRateLimit.PartitionKey)
            );

            _logger.LogAddOrUpdateOperationAdded(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
                id: lyricsAnalyzerUserRateLimit.UserId.ToString(),
                state: CosmosDbServiceLoggingConstants.OperationTypes.Updated
            );
        }
    }
}