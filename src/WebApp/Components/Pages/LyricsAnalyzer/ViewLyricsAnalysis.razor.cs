using Markdig;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

using MuzakBot.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.WebApp.Components.Pages;

[StreamRendering(false)]
public partial class ViewLyricsAnalysis : ComponentBase
{
    [Inject]
    protected IDbContextFactory<LyricsAnalyzerDbContext> LyricsAnalyzerDbContextFactory { get; set; } = null!;

    [Parameter]
    public string? AnalysisId { get; set; }

    private bool _loading = true;
    private bool _errorOccurred = false;
    private string? _errorMessage;
    private AnalyzedLyrics? _analyzedLyrics;
    private LyricsAnalyzerPromptStyle? _promptStyle;
    private string? _analyzedLyricsEnriched;

    protected override async Task OnInitializedAsync()
    {
        if (AnalysisId is null)
        {
            _loading = false;
            return;
        }

        using var dbContext = LyricsAnalyzerDbContextFactory.CreateDbContext();

        _analyzedLyrics = await dbContext.AnalyzedLyricsItems
            .FirstOrDefaultAsync(item => item.Id == AnalysisId);

        if (_analyzedLyrics is null)
        {
            _errorOccurred = true;
            _errorMessage = "Analysis not found.";
        }

        _promptStyle = await dbContext.LyricsAnalyzerPromptStyles
            .FirstOrDefaultAsync(item => item.ShortName == _analyzedLyrics!.PromptStyleUsed);

        _analyzedLyricsEnriched = Markdown.ToHtml(
            markdown: _analyzedLyrics!.Analysis,
            pipeline: new MarkdownPipelineBuilder()
                .UseGenericAttributes()
                .UseEmojiAndSmiley()
                .UseReferralLinks(
                    rels: [
                        "noopener",
                        "noreferrer"
                    ]
                )
                .UseEmphasisExtras()
                .UsePipeTables()
                .UseFootnotes()
                .UseBootstrap()
                .UseAutoLinks()
                .Build()
        );

        _loading = false;

        StateHasChanged();
    }
}