namespace MuzakBot.App.Services;

/// <summary>
/// Represents the options for the Discord service.
/// </summary>
public class DiscordServiceOptions
{
    /// <summary>
    /// Gets or sets the client token used for authentication with the Discord API.
    /// </summary>
    public string? ClientToken { get; set; }

    /// <summary>
    /// Gets or sets the ID of the test guild to use for testing purposes.
    /// </summary>
    public string? TestGuildId { get; set; }

    /// <summary>
    /// Whether or not to enable the lyrics analyzer.
    /// </summary>
    public bool EnableLyricsAnalyzer { get; set; }

    /// <summary>
    /// The ID of the guild to register admin commands to.
    /// </summary>
    public ulong AdminGuildId { get; set; }
}
