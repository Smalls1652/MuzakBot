using System.Diagnostics;
using System.Net;
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
    /// Gets the lyrics analyzer rate limit for a user from the database.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The retrieved lyrics analyzer rate limit for the user.</returns>
    public async Task<LyricsAnalyzerUserRateLimit> GetLyricsAnalyzerUserRateLimitAsync(string userId) => await GetLyricsAnalyzerUserRateLimitAsync(userId, null);

    /// <summary>
    /// Gets the lyrics analyzer rate limit for a user from the database.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="parentActivityId">The parent activity ID.</param>
    /// <returns>The retrieved lyrics analyzer rate limit for the user.</returns>
    public async Task<LyricsAnalyzerUserRateLimit> GetLyricsAnalyzerUserRateLimitAsync(string userId, string? parentActivityId)
    {
        using var activity = _activitySource.StartDbGetLyricsAnalyzerUserRateLimitActivity(
            userId: userId,
            parentActivityId: parentActivityId
        );

        _logger.LogGetOperationStart(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
            id: userId.ToString()
        );

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "rate-limit"
        );

        var query = new QueryDefinition(
            query: "SELECT * FROM c WHERE c.partitionKey = @partitionKey AND c.userId = @userId"
        )
            .WithParameter("@partitionKey", "user-item")
            .WithParameter("@userId", userId);

        CosmosDbResponse<LyricsAnalyzerUserRateLimit>? cosmosDbResponse;

        try
        {
            using FeedIterator feedIterator = container.GetItemQueryStreamIterator(
                queryDefinition: query,
                requestOptions: new()
                {
                    PartitionKey = new PartitionKey("user-item")
                }
            );

            using ResponseMessage response = await feedIterator.ReadNextAsync();

            cosmosDbResponse = await JsonSerializer.DeserializeAsync(
                utf8Json: response.Content,
                jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseLyricsAnalyzerUserRateLimit
            );
        }
        catch (Exception ex)
        {
            _logger.LogGetOperationFailed(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
                id: userId.ToString(),
                exception: ex
            );
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null || cosmosDbResponse.Documents.Length == 0)
        {
            _logger.LogGetOperationNotFound(
                itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
                id: userId.ToString()
            );

            LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit = new(
                userId: userId
            );

            await AddOrUpdateLyricsAnalyzerUserRateLimitAsync(lyricsAnalyzerUserRateLimit, parentActivityId);

            return lyricsAnalyzerUserRateLimit;
        }

        _logger.LogGetOperationFound(
            itemType: CosmosDbServiceLoggingConstants.ItemTypes.LyricsAnalyzerUserRateLimit,
            id: userId.ToString()
        );

        return cosmosDbResponse.Documents.FirstOrDefault()!;
    }
}