using System.Diagnostics.Metrics;

using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Metrics;

public class CommandMetrics
{
    private readonly string _meterName = "MuzakBot.App.Commands";

    private readonly ILogger<CommandMetrics> _logger;

    private readonly Counter<long> _shareMusicCounter;
    private readonly Counter<long> _findAlbumCounter;
    private readonly Counter<long> _findSongCounter;
    private readonly Counter<long> _getLinksFromPostCounter;

    private readonly Histogram<double> _shareMusicDuration;
    private readonly Histogram<double> _findAlbumDuration;
    private readonly Histogram<double> _findSongDuration;
    private readonly Histogram<double> _getLinksFromPostDuration;

    public CommandMetrics(ILogger<CommandMetrics> logger, IMeterFactory meterFactory)
    {
        _logger = logger;

        var meter = meterFactory.Create(_meterName);

        _shareMusicCounter = meter.CreateCounter<long>(
            name: "muzakbot.app.commands.sharemusic.count"
        );

        _findAlbumCounter = meter.CreateCounter<long>(
            name: "muzakbot.app.commands.findalbum.count"
        );

        _findSongCounter = meter.CreateCounter<long>(
            name: "muzakbot.app.commands.findsong.count"
        );

        _getLinksFromPostCounter = meter.CreateCounter<long>(
            name: "muzakbot.app.commands.getlinksfrompost.count"
        );

        _shareMusicDuration = meter.CreateHistogram<double>(
            name: "muzakbot.app.commands.sharemusic.duration",
            unit: "ms"
        );

        _findAlbumDuration = meter.CreateHistogram<double>(
            name: "muzakbot.app.commands.findalbum.duration",
            unit: "ms"
        );

        _findSongDuration = meter.CreateHistogram<double>(
            name: "muzakbot.app.commands.findsong.duration",
            unit: "ms"
        );

        _getLinksFromPostDuration = meter.CreateHistogram<double>(
            name: "muzakbot.app.commands.getlinksfrompost.duration",
            unit: "ms"
        );
    }

    public void IncrementShareMusicCounter() => _shareMusicCounter.Add(1);

    public void IncrementFindAlbumCounter() => _findAlbumCounter.Add(1);

    public void IncrementFindSongCounter() => _findSongCounter.Add(1);

    public void IncrementGetLinksFromPostCounter() => _getLinksFromPostCounter.Add(1);

    public RequestDuration<CommandMetrics> MeasureShareMusicDuration() => new(_logger, _shareMusicDuration);

    public RequestDuration<CommandMetrics> MeasureFindAlbumDuration() => new(_logger, _findAlbumDuration);

    public RequestDuration<CommandMetrics> MeasureFindSongDuration() => new(_logger, _findSongDuration);

    public RequestDuration<CommandMetrics> MeasureGetLinksFromPostDuration() => new(_logger, _getLinksFromPostDuration);
}