namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public class LyricsAnalyzerConfig : DatabaseItem, ILyricsAnalyzerConfig
{
    [JsonConstructor()]
    public LyricsAnalyzerConfig()
    {}

    public LyricsAnalyzerConfig(bool firstSetup)
    {
        if (firstSetup)
        {
            Id = Guid.NewGuid().ToString();
            PartitionKey = "lyricsanalyzer-config";
        }
    }

    [JsonPropertyName("rateLimitEnabled")]
    public bool RateLimitEnabled { get; set; }

    [JsonPropertyName("rateLimitMaxRequests")]
    public int RateLimitMaxRequests { get; set; }

    [JsonPropertyName("rateLimitIgnoredUserIds")]
    public List<ulong>? RateLimitIgnoredUserIds { get; set; }

    [JsonPropertyName("commandIsEnabledToSpecificGuilds")]
    public bool CommandIsEnabledToSpecificGuilds { get; set; }

    [JsonPropertyName("commandEnabledGuildIds")]
    public List<ulong>? CommandEnabledGuildIds { get; set; }

    [JsonPropertyName("commandDisabledGuildIds")]
    public List<ulong>? CommandDisabledGuildIds { get; set; }
}