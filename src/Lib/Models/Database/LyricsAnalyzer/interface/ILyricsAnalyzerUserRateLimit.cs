namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

public interface ILyricsAnalyzerUserRateLimit
{
    string Id { get; set; }
    string PartitionKey { get; set; }
    string UserId { get; set; }
    int CurrentRequestCount { get; set; }
    DateTimeOffset LastRequestTime { get; set; }

    void IncrementRequestCount();
    void ResetRequestCount();
    bool ShouldResetRequestCount();
    bool EvaluateRequest(int maxRequestCount);
}
