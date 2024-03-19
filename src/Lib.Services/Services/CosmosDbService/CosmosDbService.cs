using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuzakBot.Lib.Services.Logging.CosmosDb;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service for interacting with Azure Cosmos DB.
/// </summary>
public partial class CosmosDbService : ICosmosDbService
{
    private bool _isDisposed;
    private readonly ILogger<CosmosDbService> _logger;
    private readonly ActivitySource _activitySource = new("MuzakBot.Lib.Services.CosmosDbService");
    private readonly CosmosDbServiceOptions _options;
    private readonly CosmosClient _cosmosDbClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The <see cref="CosmosDbServiceOptions"/> to configure the service.</param>
    public CosmosDbService(ILogger<CosmosDbService> logger, IOptions<CosmosDbServiceOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _cosmosDbClient = new CosmosClient(_options.ConnectionString);
    }

    /// <summary>
    /// Initializes the database and containers for MuzakBot.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeDatabaseAsync()
    {
        // Initialize the database and containers for 'lyrics-analyzer',
        // if they don't exist.
        _logger.LogInitializeEnsureDbExists("lyrics-analyzer");
        Database lyricsAnalyzerDb = await _cosmosDbClient.CreateDatabaseIfNotExistsAsync(
            id: "lyrics-analyzer"
        );

        _logger.LogInitializeEnsureContainerExists("song-lyrics", "lyrics-analyzer");
        await lyricsAnalyzerDb.CreateContainerIfNotExistsAsync(
            id: "song-lyrics",
            partitionKeyPath: "/partitionKey"
        );

        _logger.LogInitializeEnsureContainerExists("command-configs", "lyrics-analyzer");
        await lyricsAnalyzerDb.CreateContainerIfNotExistsAsync(
            id: "command-configs",
            partitionKeyPath: "/partitionKey"
        );

        _logger.LogInitializeEnsureContainerExists("prompt-styles", "lyrics-analyzer");
        await lyricsAnalyzerDb.CreateContainerIfNotExistsAsync(
            id: "prompt-styles",
            partitionKeyPath: "/partitionKey"
        );

        _logger.LogInitializeEnsureContainerExists("rate-limit", "lyrics-analyzer");
        await lyricsAnalyzerDb.CreateContainerIfNotExistsAsync(
            id: "rate-limit",
            partitionKeyPath: "/partitionKey"
        );
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _cosmosDbClient.Dispose();
        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}