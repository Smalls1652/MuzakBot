using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    public async Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig) => await AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig, null);
    public async Task AddOrUpdateLyricsAnalyzerConfigAsync(LyricsAnalyzerConfig lyricsAnalyzerConfig, string? parentActivityId)
    {
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