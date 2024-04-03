using Azure.Identity;
using Azure.Storage.Queues;

using Microsoft.Extensions.Options;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service that provides a <see cref="QueueClient"/>.
/// </summary>
public sealed class QueueClientService : IQueueClientService
{
    private readonly QueueClientServiceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueueClientService"/> class.
    /// </summary>
    /// <param name="options">The <see cref="QueueClientServiceOptions"/>.</param>
    public QueueClientService(IOptions<QueueClientServiceOptions> options)
    {
        _options = options.Value;

        if (_options.ConnectionString is not null)
        {
            QueueClient = new(
                connectionString: _options.ConnectionString,
                queueName: _options.QueueName
            );
        }
        else
        {
            QueueClient = new(
                queueUri: _options.EndpointUri,
                credential: new ChainedTokenCredential([
                    new AzureCliCredential(),
                    new AzurePowerShellCredential(),
                    new ManagedIdentityCredential()
                ])
            );
        }

        QueueClient.CreateIfNotExists();
    }

    /// <inheritdoc />
    public QueueClient QueueClient { get; }
}