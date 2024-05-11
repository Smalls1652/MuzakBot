namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

public interface ILyricsAnalyzerConfig
{
    string Id { get; set; }
    string PartitionKey { get; set; }
    bool RateLimitEnabled { get; set; }
    int RateLimitMaxRequests { get; set; }
    List<string>? RateLimitIgnoredUserIds { get; set; }
    bool CommandIsEnabledToSpecificGuilds { get; set; }
    List<ulong>? CommandEnabledGuildIds { get; set; }
    List<ulong>? CommandDisabledGuildIds { get; set; }
}
