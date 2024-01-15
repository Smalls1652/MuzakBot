namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for a prompt style for the lyrics analyzer.
/// </summary>
public class LyricsAnalyzerPromptStyle : DatabaseItem, ILyricsAnalyzerPromptStyle
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerPromptStyle"/> class.
    /// </summary>
    [JsonConstructor()]
    public LyricsAnalyzerPromptStyle() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerPromptStyle"/> class.
    /// </summary>
    /// <param name="name">The name of the prompt style.</param>
    /// <param name="shortName">The short name of the prompt style.</param>
    /// <param name="description">The description of the prompt style.</param>
    /// <param name="analysisType">The analysis type of the prompt style.</param>
    /// <param name="prompt">The prompt used for the style.</param>
    /// <param name="noticeText">The notice text used for the prompt style.</param>
    public LyricsAnalyzerPromptStyle(string name, string shortName, string description, string analysisType, string prompt, string noticeText)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "prompt-style";
        Name = name;
        ShortName = shortName;
        Description = description;
        AnalysisType = analysisType;
        Prompt = prompt;
        NoticeText = noticeText;
        CreatedOn = DateTimeOffset.UtcNow;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// The name of the prompt style.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The short name of the prompt style.
    /// </summary>
    [JsonPropertyName("shortName")]
    public string ShortName { get; set; } = null!;

    /// <summary>
    /// The description of the prompt style.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;

    /// <summary>
    /// The analysis type of the prompt style.
    /// </summary>
    [JsonPropertyName("analysisType")]
    public string AnalysisType { get; set; } = null!;

    /// <summary>
    /// The prompt used for the style.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = null!;

    /// <summary>
    /// The notice text used for the prompt style.
    /// </summary>
    [JsonPropertyName("noticeText")]
    public string NoticeText { get; set; } = null!;

    /// <summary>
    /// The date and time the prompt style was created.
    /// </summary>
    [JsonPropertyName("createdOn")]
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>
    /// The date and time the prompt style was last updated.
    /// </summary>
    [JsonPropertyName("lastUpdated")]
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// Updates the last updated time to the current date and time.
    /// </summary>
    public void UpdateLastUpdateTime() => LastUpdated = DateTimeOffset.UtcNow;
}
