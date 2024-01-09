namespace MuzakBot.App.Services;

public class DiscordServiceConfig
{
    public DiscordServiceConfig(string? lyricsAnalyzerEnabledServers)
    {
        LyricsAnalyzerEnabledServers = lyricsAnalyzerEnabledServers;
    }

    public string? LyricsAnalyzerEnabledServers { get; set; }

    public string[]? LyricsAnalyzerEnabledServersArray => LyricsAnalyzerEnabledServers?.Split(';');
}