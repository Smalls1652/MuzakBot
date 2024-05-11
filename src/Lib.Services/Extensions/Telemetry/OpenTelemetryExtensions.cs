using Microsoft.Extensions.Diagnostics.Metrics;

using OpenTelemetry.Trace;

namespace MuzakBot.Lib.Services.Extensions.Telemetry;

public static class OpenTelemetryExtensions
{
    public static TracerProviderBuilder AddMuzakBotServicesTraceSources(this TracerProviderBuilder builder)
    {
        return builder
            .AddSource("MuzakBot.Lib.Services.ItunesApiService")
            .AddSource("MuzakBot.Lib.Services.MusicBrainzService")
            .AddSource("MuzakBot.Lib.Services.OdesliService")
            .AddSource("MuzakBot.Lib.Services.OpenAiService")
            .AddSource("MuzakBot.Lib.Service.GeniusApiService")
            .AddSource("MuzakBot.Lib.Services.CosmosDbService");
    }
}
