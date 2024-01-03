using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Contains extension methods for configuring OpenTelemetry instrumentation.
/// </summary>
internal static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds command metrics instrumentation to the see cref="MeterProviderBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="MeterProviderBuilder"/> to add the instrumentation to.</param>
    /// <returns>The see cref="MeterProviderBuilder"/> with the added instrumentation.</returns>
    public static MeterProviderBuilder AddCommandMetricsInstrumentation(this MeterProviderBuilder builder)
    {
        return builder.AddMeter("MuzakBot.App.Commands");
    }

    /// <summary>
    /// Adds MuzakBot tracer sources to the <see cref="TracerProviderBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="TracerProviderBuilder"/> to add the tracer sources to.</param>
    /// <returns>The <see cref="TracerProviderBuilder"/> with the added tracer sources.</returns>
    public static TracerProviderBuilder AddMuzakBotTracerSources(this TracerProviderBuilder builder)
    {
        return builder
            .AddSource("MuzakBot.App.Modules.ShareMusicCommandModule")
            .AddSource("MuzakBot.App.Services.DiscordService")
            .AddSource("MuzakBot.App.Services.ItunesApiService")
            .AddSource("MuzakBot.App.Services.MusicBrainzService")
            .AddSource("MuzakBot.App.Services.OdesliService");
    }
}