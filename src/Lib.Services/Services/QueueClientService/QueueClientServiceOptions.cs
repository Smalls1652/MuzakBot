namespace MuzakBot.Lib.Services;

/// <summary>
/// Options for configuring <see cref="QueueClientService"/>.
/// </summary>
public sealed class QueueClientServiceOptions
{
    /// <summary>
    /// The endpoint URI for the Azure Queue.
    /// </summary>
    /// <remarks>
    /// Not required if <see cref="ConnectionString"/> is provided.
    /// </remarks>
    public Uri? EndpointUri { get; set; }

    /// <summary>
    /// The name of the queue.
    /// </summary>
    public string QueueName { get; set; } = null!;

    /// <summary>
    /// The connection string for the Azure Queue.
    /// </summary>
    /// <remarks>
    /// Not required if <see cref="EndpointUri"/> is provided.
    /// </remarks>
    public string? ConnectionString { get; set; }
}