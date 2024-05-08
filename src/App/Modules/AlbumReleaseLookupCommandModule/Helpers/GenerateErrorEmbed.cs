using Discord;

namespace MuzakBot.App.Modules;

public partial class AlbumReleaseLookupCommandModule
{
    private EmbedBuilder GenerateErrorEmbed(string title, string message)
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(message)
            .WithColor(Color.Red);
    }

    private EmbedBuilder GenerateErrorEmbed(string message) => GenerateErrorEmbed("💥 An error occurred", message);
}