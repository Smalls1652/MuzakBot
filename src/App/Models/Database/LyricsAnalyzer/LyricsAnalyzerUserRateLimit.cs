namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for a user's rate limit for the lyrics analyzer.
/// </summary>
public class LyricsAnalyzerUserRateLimit : DatabaseItem, ILyricsAnalyzerUserRateLimit
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerUserRateLimit"/> class.
    /// </summary>
    [JsonConstructor()]
    public LyricsAnalyzerUserRateLimit()
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerUserRateLimit"/> class.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    public LyricsAnalyzerUserRateLimit(ulong userId): this(userId, 0, DateTimeOffset.UtcNow)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerUserRateLimit"/> class.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="currentRequestCount">The current request count.</param>
    public LyricsAnalyzerUserRateLimit(ulong userId, int currentRequestCount): this(userId, currentRequestCount, DateTimeOffset.UtcNow)
    {}

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerUserRateLimit"/> class.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <param name="currentRequestCount">The current request count.</param>
    /// <param name="lastRequestTime">The last request time.</param>
    public LyricsAnalyzerUserRateLimit(ulong userId, int currentRequestCount, DateTimeOffset lastRequestTime)
    {
        UserId = userId;
        CurrentRequestCount = currentRequestCount;
        LastRequestTime = lastRequestTime;
    }

    /// <summary>
    /// The user's ID.
    /// </summary>
    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The current request count.
    /// </summary>
    [JsonPropertyName("currentRequestCount")]
    public int CurrentRequestCount { get; set; }

    /// <summary>
    /// The last request time.
    /// </summary>
    [JsonPropertyName("lastRequestTime")]
    public DateTimeOffset LastRequestTime { get; set; }

    /// <summary>
    /// Increments the request count and sets the last request time to the current time.
    /// </summary>
    public void IncrementRequestCount()
    {
        CurrentRequestCount++;
        LastRequestTime = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Resets the request count to 0.
    /// </summary>
    public void ResetRequestCount()
    {
        CurrentRequestCount = 0;
    }
}