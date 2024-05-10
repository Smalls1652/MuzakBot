using Discord;

namespace MuzakBot.App.Modules;

public partial class AlbumReleaseCommandModule
{
    /// <summary>
    /// Generates an error embed.
    /// </summary>
    /// <param name="title">The title of the error embed.</param>
    /// <param name="message">The message of the error embed.</param>
    /// <returns>An embed builder for the error embed.</returns>
    private EmbedBuilder GenerateErrorEmbed(string title, string message)
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(message)
            .WithColor(Color.Red);
    }

    /// <summary>
    /// Generates an error embed.
    /// </summary>
    /// <param name="message">The message of the error embed.</param>
    /// <returns>An embed builder for the error embed.</returns>
    private EmbedBuilder GenerateErrorEmbed(string message) => GenerateErrorEmbed("💥 An error occurred", message);
}