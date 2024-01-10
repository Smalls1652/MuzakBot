using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace MuzakBot.App.Services;

/// <summary>
/// Extension methods for adding Discord service to the service collection.
/// </summary>
public static class DiscordServiceExtensions
{
    /// <summary>
    /// Adds the Discord service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An action to configure the Discord service options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDiscordService(this IServiceCollection services, Action<DiscordServiceOptions> configure)
    {
        GatewayIntents gatewayIntents = GatewayIntents.AllUnprivileged - GatewayIntents.GuildInvites - GatewayIntents.GuildScheduledEvents;

#if DEBUG
        DiscordSocketConfig discordSocketConfig = new()
        {
            GatewayIntents = gatewayIntents,
            UseInteractionSnowflakeDate = false
        };
#else
        DiscordSocketConfig discordSocketConfig = new()
        {
            GatewayIntents = gatewayIntents
        };
#endif

        services.Configure(configure);

        services.AddSingleton<DiscordSocketClient>(
            implementationInstance: new(discordSocketConfig)
        );

        services.AddSingleton<InteractionService>(serviceProvider => new(serviceProvider.GetRequiredService<DiscordSocketClient>()));

        services.AddHostedService<DiscordService>();

        return services;
    }
}