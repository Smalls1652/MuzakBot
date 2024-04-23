using System.ComponentModel.DataAnnotations.Schema;

using MuzakBot.Lib.Models.CommandModules;

namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

/// <summary>
/// Holds data for a prompt style for the lyrics analyzer.
/// </summary>
[Table("prompt_styles")]
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
    /// <param name="inputModal">The <see cref="LyricsAnalyzerPromptStyleModal"/> to initialize from.</param>
    public LyricsAnalyzerPromptStyle(LyricsAnalyzerPromptStyleModal inputModal)
    {
        Id = Guid.NewGuid().ToString();
        PartitionKey = "prompt-style";
        UpdateFromModalInput(inputModal);
        CreatedOn = DateTimeOffset.UtcNow;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerPromptStyle"/> class.
    /// </summary>
    /// <param name="name">The name of the prompt style.</param>
    /// <param name="shortName">The short name of the prompt style.</param>
    /// <param name="description">The description of the prompt style.</param>
    /// <param name="analysisType">The analysis type of the prompt style.</param>
    /// <param name="prompt">The prompt used for the style.</param>
    /// <param name="noticeText">The notice text used for the prompt style.</param>
    public LyricsAnalyzerPromptStyle(string name, string shortName, string analysisType, string prompt, string noticeText)
    {
        Name = name;
        ShortName = shortName;
        AnalysisType = analysisType;
        Prompt = prompt;
        NoticeText = noticeText;
        CreatedOn = DateTimeOffset.UtcNow;
        LastUpdated = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// The name of the prompt style.
    /// </summary>
    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The short name of the prompt style.
    /// </summary>
    [Column("shortName")]
    [JsonPropertyName("shortName")]
    public string ShortName { get; set; } = null!;

    /// <summary>
    /// The analysis type of the prompt style.
    /// </summary>
    [Column("analysisType")]
    [JsonPropertyName("analysisType")]
    public string AnalysisType { get; set; } = null!;

    /// <summary>
    /// The prompt used for the style.
    /// </summary>
    [Column("prompt")]
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = null!;

    /// <summary>
    /// The prompt used as a user prompt for the style.
    /// </summary>
    [Column("userPrompt")]
    [JsonPropertyName("userPrompt")]
    public string UserPrompt { get; set; } = "Briefly explain the lyrics for the song \"{{songName}}\" by {{artistName}}. Format the response in Markdown syntax.";

    /// <summary>
    /// The notice text used for the prompt style.
    /// </summary>
    [Column("noticeText")]
    [JsonPropertyName("noticeText")]
    public string NoticeText { get; set; } = null!;

    /// <summary>
    /// The date and time the prompt style was created.
    /// </summary>
    [Column("createdOn")]
    [JsonPropertyName("createdOn")]
    public DateTimeOffset CreatedOn { get; set; }

    /// <summary>
    /// The date and time the prompt style was last updated.
    /// </summary>
    [Column("lastUpdated")]
    [JsonPropertyName("lastUpdated")]
    public DateTimeOffset LastUpdated { get; set; }

    /// <summary>
    /// Updates the prompt style from <see cref="LyricsAnalyzerPromptStyleModal"/>.
    /// </summary>
    /// <param name="inputModal">The <see cref="LyricsAnalyzerPromptStyleModal"/> to update from.</param>
    public void UpdateFromModalInput(LyricsAnalyzerPromptStyleModal inputModal)
    {
        Name = inputModal.Name;
        ShortName = inputModal.ShortName;
        AnalysisType = inputModal.AnalysisType;
        Prompt = inputModal.Prompt;
        NoticeText = inputModal.NoticeText;
    }

    /// <summary>
    /// Updates the last updated time to the current date and time.
    /// </summary>
    public void UpdateLastUpdateTime() => LastUpdated = DateTimeOffset.UtcNow;
}
