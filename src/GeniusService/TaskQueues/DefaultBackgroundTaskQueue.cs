using System.Threading.Channels;

namespace MuzakBot.GeniusService.TaskQueues;

/// <summary>
/// Default implementation of <see cref="IBackgroundTaskQueue"/>.
/// </summary>
public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBackgroundTaskQueue"/> class.
    /// </summary>
    /// <param name="capacity">The capacity of the queue.</param>
    public DefaultBackgroundTaskQueue(int capacity)
    {
        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    /// <inheritdoc />
    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        await _queue.Writer.WriteAsync(workItem);
    }

    /// <inheritdoc />
    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        Func<CancellationToken, ValueTask>? workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}