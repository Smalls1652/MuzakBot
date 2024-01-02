using OpenTelemetry.Metrics;

namespace MuzakBot.App.Metrics;

public static class CommandMetricsMeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddCommandMetricsInstrumentation(this MeterProviderBuilder builder)
    {
        return builder.AddMeter("MuzakBot.App.Commands");
    }
}