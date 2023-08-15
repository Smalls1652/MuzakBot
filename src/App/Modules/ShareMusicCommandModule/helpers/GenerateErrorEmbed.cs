using Discord;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    private EmbedBuilder GenerateErrorEmbed(string message, string? title = "💥 An error occurred")
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(message)
            .WithColor(Color.Red);
    }
}