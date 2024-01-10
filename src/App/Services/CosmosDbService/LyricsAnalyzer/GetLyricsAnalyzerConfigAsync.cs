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
            containerId: "config"
        );

        var query = new QueryDefinition(
            query: "SELECT * FROM c WHERE c.partitionKey = 'lyricsanalyzer-config'"
        );

        using FeedIterator feedIterator = container.GetItemQueryStreamIterator(
            queryDefinition: query,
            requestOptions: new()
            {
                PartitionKey = new PartitionKey("lyricsanalyzer-config")
            }
        );

        using ResponseMessage response = await feedIterator.ReadNextAsync();

        CosmosDbResponse<LyricsAnalyzerConfig>? cosmosDbResponse = await JsonSerializer.DeserializeAsync(
            utf8Json: response.Content,
            jsonTypeInfo: DatabaseJsonContext.Default.CosmosDbResponseLyricsAnalyzerConfig
        );

        if (cosmosDbResponse is null || cosmosDbResponse.Documents is null || cosmosDbResponse.Documents.Length == 0)
        {
            LyricsAnalyzerConfig lyricsAnalyzerConfig = new(true);

            await AddOrUpdateLyricsAnalyzerConfigAsync(lyricsAnalyzerConfig);

            return lyricsAnalyzerConfig;
        }

        return cosmosDbResponse.Documents.FirstOrDefault()!;
    }
}