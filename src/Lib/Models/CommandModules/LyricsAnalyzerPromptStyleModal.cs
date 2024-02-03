using Discord;
using Discord.Interactions;
using MuzakBot.Lib.Models.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Lib.Models.CommandModules;

public class LyricsAnalyzerPromptStyleModal : IModal
{
    public LyricsAnalyzerPromptStyleModal()
    {}

    public LyricsAnalyzerPromptStyleModal(LyricsAnalyzerPromptStyle promptStyle)
    {
        Name = promptStyle.Name;
        ShortName = promptStyle.ShortName;
        AnalysisType = promptStyle.AnalysisType;
        Prompt = promptStyle.Prompt;
        NoticeText = promptStyle.NoticeText;
    }

    public string Title => "Lyrics Analyzer Prompt Style";

    /// <summary>
    /// The name of the prompt style.
    /// </summary>
    [JsonPropertyName("name")]
    [InputLabel("Name")]
    [ModalTextInput(
        customId: "prompt-style-name",
        placeholder: "Enter the name of the prompt style",
        minLength: 1,
        maxLength: 100,
        style: TextInputStyle.Short
    )]
    public string Name { get; set; } = null!;

    /// <summary>
    /// The short name of the prompt style.
    /// </summary>
    [JsonPropertyName("shortName")]
    [InputLabel("Short Name")]
    [ModalTextInput(
        customId: "prompt-style-short-name",
        placeholder: "Enter the short name of the prompt style",
        minLength: 1,
        maxLength: 50,
        style: TextInputStyle.Short
    )]
    public string ShortName { get; set; } = null!;

    /// <summary>
    /// The analysis type of the prompt style.
    /// </summary>
    [JsonPropertyName("analysisType")]
    [InputLabel("Analysis Type")]
    [ModalTextInput(
        customId: "prompt-style-analysis-type",
        placeholder: "Enter the analysis type of the prompt style",
        minLength: 1,
        maxLength: 100,
        style: TextInputStyle.Short
    )]
    public string AnalysisType { get; set; } = null!;

    /// <summary>
    /// The prompt used for the style.
    /// </summary>
    [JsonPropertyName("prompt")]
    [InputLabel("Prompt")]
    [ModalTextInput(
        customId: "prompt-style-prompt",
        placeholder: "Enter the prompt used for the style",
        minLength: 1,
        maxLength: 4000,
        style: TextInputStyle.Paragraph
    )]
    public string Prompt { get; set; } = null!;

    /// <summary>
    /// The notice text used for the prompt style.
    /// </summary>
    [JsonPropertyName("noticeText")]
    [InputLabel("Notice Text")]
    [ModalTextInput(
        customId: "prompt-style-notice-text",
        placeholder: "Enter the notice text used for the prompt style",
        minLength: 1,
        maxLength: 4000,
        style: TextInputStyle.Paragraph
    )]
    public string NoticeText { get; set; } = null!;
}