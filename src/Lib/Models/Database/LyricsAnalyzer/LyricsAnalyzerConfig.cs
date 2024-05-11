using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds the configuration for the lyrics analyzer.
/// </summary>
[Table("command_configs")]
public class LyricsAnalyzerConfig : DatabaseItem, ILyricsAnalyzerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerConfig"/> class.
    /// </summary>
    [JsonConstructor()]
    public LyricsAnalyzerConfig()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerConfig"/> class.
    /// </summary>
    /// <param name="firstSetup">Whether or not this is the first time the config is being setup.</param>
    public LyricsAnalyzerConfig(bool firstSetup)
    {
        if (firstSetup)
        {
            Id = Guid.NewGuid().ToString();
            PartitionKey = "lyricsanalyzer-config";
        }
    }

    /// <summary>
    /// Whether or not the lyrics analyzer is enabled.
    /// </summary>
    [Column("rateLimitEnabled")]
    [JsonPropertyName("rateLimitEnabled")]
    public bool RateLimitEnabled { get; set; }

    /// <summary>
    /// The number of requests allowed per rate limit period.
    /// </summary>
    [Column("rateLimitMaxRequests")]
    [JsonPropertyName("rateLimitMaxRequests")]
    public int RateLimitMaxRequests { get; set; }

    /// <summary>
    /// Users to ignore when rate limiting.
    /// </summary>
    [Column("rateLimitIgnoredUserIds")]
    [JsonPropertyName("rateLimitIgnoredUserIds")]
    public List<string>? RateLimitIgnoredUserIds { get; set; }

    /// <summary>
    /// Whether or not the lyrics analyzer is enabled only to specific guilds/servers.
    /// </summary>
    [Column("commandIsEnabledToSpecificGuilds")]
    [JsonPropertyName("commandIsEnabledToSpecificGuilds")]
    public bool CommandIsEnabledToSpecificGuilds { get; set; }

    /// <summary>
    /// Guild (server) IDs that the lyrics analyzer is enabled for.
    /// </summary>
    [Column("commandEnabledGuildIds")]
    [JsonPropertyName("commandEnabledGuildIds")]
    public List<ulong>? CommandEnabledGuildIds { get; set; }

    /// <summary>
    /// Guild (server) IDs that the lyrics analyzer is disabled for.
    /// </summary>
    [Column("commandDisabledGuildIds")]
    [JsonPropertyName("commandDisabledGuildIds")]
    public List<ulong>? CommandDisabledGuildIds { get; set; }
}
