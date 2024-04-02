using Microsoft.Extensions.DependencyInjection;

using MuzakBot.GeniusService.Services;

namespace MuzakBot.GeniusService.Extensions;

internal static partial class ServiceSetupExtensions
{
    public static IServiceCollection AddMainService(this IServiceCollection services) => AddMainService(services, _ => { });
    
    public static IServiceCollection AddMainService(this IServiceCollection services, Action<MainServiceOptions> configure)
    {
        services.Configure(configure);

        services.AddHostedService<MainService>();;

        return services;
    }
}
