using Discord;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Generates an error embed with the given title and message.
    /// </summary>
    /// <param name="title">The title of the embed.</param>
    /// <param name="message">The message of the embed.</param>
    /// <returns>The <see cref="EmbedBuilder"/> for the error embed.</returns>
    private EmbedBuilder GenerateErrorEmbed(string title, string message)
    {
        return new EmbedBuilder()
            .WithTitle(title)
            .WithDescription(message)
            .WithColor(Color.Red);
    }

    /// <summary>
    /// Generates an error embed with the given message.
    /// </summary>
    /// <param name="message">The message of the embed.</param>
    /// <returns>The <see cref="EmbedBuilder"/> for the error embed.</returns>
    private EmbedBuilder GenerateErrorEmbed(string message) => GenerateErrorEmbed("💥 An error occurred", message);
}
