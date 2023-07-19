using Discord;

namespace MuzakBot.App.Services;

public interface IDiscordService
{
    Task ConnectAsync();
}