using Discord;
using Discord.Interactions;

namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public class LyricsAnalyzerPromptStyleUserPromptModal : IModal
{
    public LyricsAnalyzerPromptStyleUserPromptModal()
    { }

    public LyricsAnalyzerPromptStyleUserPromptModal(LyricsAnalyzerPromptStyle promptStyle)
    {
        ShortName = promptStyle.ShortName;
        UserPrompt = promptStyle.UserPrompt;
    }

    public string Title => "Lyrics Analyzer Prompt Style";

    [JsonPropertyName("shortName")]
    [InputLabel("Short name")]
    [ModalTextInput(
        customId: "prompt-style-short-name",
        placeholder: "Enter the short name of the prompt style",
        minLength: 1,
        maxLength: 50,
        style: TextInputStyle.Short
    )]
    public string ShortName { get; set; } = null!;

    [JsonPropertyName("userPrompt")]
    [InputLabel("User prompt")]
    [ModalTextInput(
        customId: "prompt-style-user-prompt",
        placeholder: "Enter the user prompt of the prompt style",
        minLength: 1,
        maxLength: 4000,
        style: TextInputStyle.Paragraph
    )]
    public string UserPrompt { get; set; } = null!;
}