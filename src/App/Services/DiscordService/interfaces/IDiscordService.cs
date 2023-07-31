using Discord;

namespace MuzakBot.App.Services;

/// <summary>
/// Interface for the Discord service.
/// </summary>
public interface IDiscordService
{
    /// <summary>
    /// Connects the bot to Discord.
    /// </summary>
    /// <returns></returns>
    Task ConnectAsync();
}