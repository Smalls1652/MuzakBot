namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public class LyricsAnalyzerUserRateLimit : DatabaseItem, ILyricsAnalyzerUserRateLimit
{
    [JsonConstructor()]
    public LyricsAnalyzerUserRateLimit()
    {}

    public LyricsAnalyzerUserRateLimit(ulong userId): this(userId, 0, DateTimeOffset.UtcNow)
    {}

    public LyricsAnalyzerUserRateLimit(ulong userId, int currentRequestCount): this(userId, currentRequestCount, DateTimeOffset.UtcNow)
    {}

    public LyricsAnalyzerUserRateLimit(ulong userId, int currentRequestCount, DateTimeOffset lastRequestTime)
    {
        UserId = userId;
        CurrentRequestCount = currentRequestCount;
        LastRequestTime = lastRequestTime;
    }

    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    [JsonPropertyName("currentRequestCount")]
    public int CurrentRequestCount { get; set; }

    [JsonPropertyName("lastRequestTime")]
    public DateTimeOffset LastRequestTime { get; set; }

    public void IncrementRequestCount()
    {
        CurrentRequestCount++;
        LastRequestTime = DateTimeOffset.UtcNow;
    }

    public void ResetRequestCount()
    {
        CurrentRequestCount = 0;
    }
}