namespace MuzakBot.Lib.Services;

public sealed class QueueClientServiceOptions
{
    public Uri? EndpointUri { get; set; }

    public string QueueName { get; set; } = null!;

    public string? ConnectionString { get; set; }
}