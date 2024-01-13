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

    public bool EnableLyricsAnalyzer { get; set; }

    public ulong AdminGuildId { get; set; }
}