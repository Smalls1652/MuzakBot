using System.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MuzakBot.App.Services;

public partial class CosmosDbService : ICosmosDbService
{
    private bool _isDisposed;
    private readonly ILogger<CosmosDbService> _logger;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.CosmosDbService");
    private readonly CosmosDbServiceOptions _options;
    private readonly CosmosClient _cosmosDbClient;

    public CosmosDbService(ILogger<CosmosDbService> logger, IOptions<CosmosDbServiceOptions> options)
    {
        _logger = logger;
        _options = options.Value;
        _cosmosDbClient = new CosmosClient(_options.ConnectionString);
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