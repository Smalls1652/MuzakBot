using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

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

        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "rate-limit"
        );

        _logger.LogInformation("Serializing lyrics analyzer user rate limit.");
        using MemoryStream itemPayload = new();
        await JsonSerializer.SerializeAsync(
            utf8Json: itemPayload,
            value: lyricsAnalyzerUserRateLimit,
            jsonTypeInfo: DatabaseJsonContext.Default.LyricsAnalyzerUserRateLimit
        );

        _logger.LogInformation("Pushing to the database.");

        try
        {
            await container.UpsertItemStreamAsync(
                streamPayload: itemPayload,
                partitionKey: new PartitionKey(lyricsAnalyzerUserRateLimit.PartitionKey)
            );

            _logger.LogInformation("Lyrics analyzer user rate limit added.");
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            await container.ReplaceItemStreamAsync(
                streamPayload: itemPayload,
                id: lyricsAnalyzerUserRateLimit.Id,
                partitionKey: new PartitionKey(lyricsAnalyzerUserRateLimit.PartitionKey)
            );

            _logger.LogInformation("Lyrics analyzer user rate limit updated.");
        }
    }
}