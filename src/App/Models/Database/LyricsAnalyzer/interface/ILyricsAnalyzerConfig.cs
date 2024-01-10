namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public interface ILyricsAnalyzerConfig
{
    string Id { get; set; }
    string PartitionKey { get; set; }
    bool RateLimitEnabled { get; set; }
    int RateLimitMaxRequests { get; set; }
    ulong[]? RateLimitIgnoredUserIds { get; set; }
    bool CommandIsEnabledToSpecificGuilds { get; set; }
    ulong[]? CommandEnabledGuildIds { get; set; }
    ulong[]? CommandDisabledGuildIds { get; set; }
}