using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.CosmosDb;
using MuzakBot.App.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    public async Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync() => await GetLyricsAnalyzerConfigAsync(null);
    public async Task<LyricsAnalyzerConfig> GetLyricsAnalyzerConfigAsync(string? parentActivityId)
    {
        Container container = _cosmosDbClient.GetContainer(
            databaseId: _options.DatabaseName,
            containerId: "command-configs"
        );

        _logger.LogInformation("Getting lyrics analyzer config from the database.");
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
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            throw;
        }

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null || cosmosDbResponse.Documents.Length == 0)
        {
            _logger.LogWarning("Lyrics analyzer config not found. Creating a new one.");
            LyricsAnalyzerConfig lyricsAnalyzerConfig = new(true);

            try
            {
                await AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                throw;
            }

            return lyricsAnalyzerConfig;
        }

        return cosmosDbResponse.Documents.FirstOrDefault()!;
    }
}