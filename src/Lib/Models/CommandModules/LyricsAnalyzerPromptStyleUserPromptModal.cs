using Discord;
using Discord.Interactions;

namespace MuzakBot.Lib.Models.Database.LyricsAnalyzer;

public class LyricsAnalyzerPromptStyleUserPromptModal : IModal
{
    public LyricsAnalyzerPromptStyleUserPromptModal()
    { }

    public LyricsAnalyzerPromptStyleUserPromptModal(LyricsAnalyzerPromptStyle promptStyle)
    {
        ShortName = promptStyle.ShortName;
        GptModel = promptStyle.GptModel;
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

    [JsonPropertyName("gptModel")]
    [InputLabel("GPT model")]
    [ModalTextInput(
        customId: "prompt-style-gpt-model",
        placeholder: "Enter the GPT model of the prompt style",
        minLength: 1,
        maxLength: 100,
        style: TextInputStyle.Short
    )]
    public string GptModel { get; set; } = "gpt-4o";

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
