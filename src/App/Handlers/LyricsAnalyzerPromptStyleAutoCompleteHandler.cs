using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.Itunes;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Handlers;

public class LyricsAnalyzerPromptStyleAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<LyricsAnalyzerPromptStyleAutoCompleteHandler> _logger;
    private readonly IDbContextFactory<LyricsAnalyzerContext> _lyricsAnalyzerContextFactory;

    public LyricsAnalyzerPromptStyleAutoCompleteHandler(ILogger<LyricsAnalyzerPromptStyleAutoCompleteHandler> logger, IDbContextFactory<LyricsAnalyzerContext> lyricsAnalyzerContextFactory)
    {
        _logger = logger;
        _lyricsAnalyzerContextFactory = lyricsAnalyzerContextFactory;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        using var lyricsAnalyzerContext = _lyricsAnalyzerContextFactory.CreateDbContext();

        List<AutocompleteResult> results = new();

        LyricsAnalyzerPromptStyle[] promptStyles = await lyricsAnalyzerContext.PromptStyles.ToArrayAsync();
        Array.Sort(promptStyles, (item1, item2) => item1.Name.CompareTo(item2.Name));

        _logger.LogInformation("Returned {Count} prompt styles from the database.", promptStyles.Length);

        AutocompleteOption? promptStyleInput = autocompleteInteraction.Data.Options.FirstOrDefault(item => item.Name == "style");

        if (promptStyleInput is null || promptStyleInput.Value is null || string.IsNullOrWhiteSpace(promptStyleInput.Value.ToString()))
        {
            _logger.LogInformation("Prompt style input was null or empty, returning all prompt styles.");

            foreach (LyricsAnalyzerPromptStyle promptStyle in promptStyles)
            {
                results.Add(new AutocompleteResult
                {
                    Name = promptStyle.Name,
                    Value = promptStyle.ShortName
                });
            }

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        promptStyles
            .Where(item => item.ShortName.Contains(promptStyleInput.Value.ToString()!, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(item => results.Add(new AutocompleteResult
            {
                Name = item.Name,
                Value = item.ShortName
            }));

        _logger.LogInformation("Filtered down to {Count} prompt styles based off the input {input}.", results.Count, promptStyleInput.Value.ToString()!);

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}