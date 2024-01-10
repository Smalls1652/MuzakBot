namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public class LyricsAnalyzerConfig : DatabaseItem, ILyricsAnalyzerConfig
{
    [JsonConstructor()]
    public LyricsAnalyzerConfig()
    {}

    public bool RateLimitEnabled { get; set; }
    public int RateLimitMaxRequests { get; set; }
    public ulong[]? RateLimitIgnoredUserIds { get; set; }
    public bool CommandIsEnabledToSpecificGuilds { get; set; }
    public ulong[]? CommandEnabledGuildIds { get; set; }
    public ulong[]? CommandDisabledGuildIds { get; set; }
}