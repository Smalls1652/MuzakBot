using Discord;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Interface for a response to a bot interaction.
/// </summary>
public interface IBotResponse
{
    /// <summary>
    /// Generates the text for the response.
    /// </summary>
    /// <returns>The response text.</returns>
    string GenerateText();

    /// <summary>
    /// Generates a component for the response.
    /// </summary>
    /// <returns>The <see cref="ComponentBuilder"/> for the response.</returns>
    ComponentBuilder GenerateComponent();

    /// <summary>
    /// Generates an embed for the response.
    /// </summary>
    /// <returns>The <see cref="EmbedBuilder"/> for the response.</returns>
    EmbedBuilder GenerateEmbed();
}
