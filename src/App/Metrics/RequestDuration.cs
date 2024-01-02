using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Metrics;

public class RequestDuration<T> : IDisposable
{
    private readonly long _requestStartTime = TimeProvider.System.GetTimestamp();
    
    private readonly ILogger<T> _logger;
    private readonly Histogram<double> _histogram;

    public RequestDuration(ILogger<T> logger, Histogram<double> histogram)
    {
        _logger = logger;
        _histogram = histogram;
    }

    public void Dispose()
    {
        TimeSpan elapsedTime = TimeProvider.System.GetElapsedTime(_requestStartTime);

        _logger.LogInformation("[{histogramName}] Request took {elapsedTime} ms.", _histogram.Name, elapsedTime.TotalMilliseconds);

        _histogram.Record(elapsedTime.TotalMilliseconds);
    }
}