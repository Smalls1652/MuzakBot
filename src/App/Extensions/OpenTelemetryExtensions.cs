using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.ResourceDetectors.Container;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Contains extension methods for configuring OpenTelemetry instrumentation.
/// </summary>
internal static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds OpenTelemetry logging to the <see cref="ILoggingBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the logging to.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> with the added OpenTelemetry logging.</returns>
    public static ILoggingBuilder AddOpenTelemetryLogging(this ILoggingBuilder builder) => AddOpenTelemetryLogging(builder, null);

    /// <summary>
    /// Adds OpenTelemetry logging to the <see cref="ILoggingBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to add the logging to.</param>
    /// <param name="azureAppInsightsConnectionString">The Azure Application Insights connection string.</param>
    /// <returns>The <see cref="ILoggingBuilder"/> with the added OpenTelemetry logging.</returns>
    public static ILoggingBuilder AddOpenTelemetryLogging(this ILoggingBuilder builder, string? azureAppInsightsConnectionString)
    {
        builder.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;
            logging.IncludeFormattedMessage = true;

            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService("MuzakBot");

            logging
                .SetResourceBuilder(resourceBuilder)
                .AddConsoleExporter()
                .AddOtlpExporter();

            if (azureAppInsightsConnectionString is not null)
            {
                logging.AddAzureMonitorLogExporter(options =>
                {
                    options.ConnectionString = azureAppInsightsConnectionString;
                });
            }
        });

        return builder;
    }

    /// <summary>
    /// Adds OpenTelemetry metrics and tracing to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the metrics and tracing to.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the added metrics and tracing.</returns>
    public static IServiceCollection AddOpenTelemetryMetricsAndTracing(this IServiceCollection services) => AddOpenTelemetryMetricsAndTracing(services, null);

    /// <summary>
    /// Adds OpenTelemetry metrics and tracing to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the metrics and tracing to.</param>
    /// <param name="azureAppInsightsConnectionString">The Azure Application Insights connection string.</param>
    /// <returns>The <see cref="IServiceCollection"/> with the added metrics and tracing.</returns>
    public static IServiceCollection AddOpenTelemetryMetricsAndTracing(this IServiceCollection services, string? azureAppInsightsConnectionString)
    {
        services.AddMetrics();

        services.AddSingleton<CommandMetrics>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resourceBuilder => resourceBuilder.AddService("MuzakBot"))
            .WithMetrics(metrics =>
            {
                var resourceBuilder = ResourceBuilder
                    .CreateDefault()
                    .AddService("MuzakBot");

                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddCommandMetricsInstrumentation();

                metrics.AddOtlpExporter();

                if (azureAppInsightsConnectionString is not null)
                {
                    metrics.AddAzureMonitorMetricExporter(options =>
                    {
                        options.ConnectionString = azureAppInsightsConnectionString;
                    });
                }
            })
            .WithTracing(tracing =>
            {
                var resourceBuilder = ResourceBuilder
                    .CreateDefault()
                    .AddService("MuzakBot")
                    .AddDetector(new ContainerResourceDetector());
            
                tracing
                    .AddMuzakBotTracerSources()
                    .SetResourceBuilder(resourceBuilder)
                    .AddHttpClientInstrumentation();

                tracing.AddOtlpExporter();

                if (azureAppInsightsConnectionString is not null)
                {
                    tracing.AddAzureMonitorTraceExporter(options =>
                    {
                        options.ConnectionString = azureAppInsightsConnectionString;
                    });
                }
            });

        return services;
    }

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