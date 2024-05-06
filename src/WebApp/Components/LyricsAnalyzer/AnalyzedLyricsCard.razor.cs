using Microsoft.AspNetCore.Components;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.WebApp.Components.LyricsAnalyzer;

public partial class AnalyzedLyricsCard : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public AnalyzedLyrics AnalyzedLyrics { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public LyricsAnalyzerPromptStyle PromptStyle { get; set; } = null!;

    [Parameter]
    [EditorRequired]
    public string AnalysisMarkdown { get; set; } = null!;
}