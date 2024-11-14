using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MuzakBot.Database;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Handlers;

public class LyricsAnalyzerPromptStyleAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<LyricsAnalyzerPromptStyleAutoCompleteHandler> _logger;
    private readonly IDbContextFactory<MuzakBotDbContext> _muzakbotDbContextFactory;

    public LyricsAnalyzerPromptStyleAutoCompleteHandler(ILogger<LyricsAnalyzerPromptStyleAutoCompleteHandler> logger, IDbContextFactory<MuzakBotDbContext> muzakbotDbContextFactory)
    {
        _logger = logger;
        _muzakbotDbContextFactory = muzakbotDbContextFactory;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        AutocompleteOption? promptStyleInput = autocompleteInteraction.Data.Options.FirstOrDefault(item => item.Name == "style");

        string? promptStyleInputValue = promptStyleInput?.Value.ToString();

        using var dbContext = _muzakbotDbContextFactory.CreateDbContext();

        LyricsAnalyzerPromptStyle[] promptStyles;

        if (string.IsNullOrEmpty(promptStyleInputValue))
        {
            promptStyles = await dbContext.LyricsAnalyzerPromptStyles
                .AsNoTracking()                .ToArrayAsync();
        }
        else
        {
            promptStyles = dbContext.LyricsAnalyzerPromptStyles
                .AsNoTracking()                .AsEnumerable()
                .Where(item => item.Name.Contains(promptStyleInputValue, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        Array.Sort(promptStyles, (item1, item2) => item1.Name.CompareTo(item2.Name));

        _logger.LogInformation("Returned {Count} prompt styles from the database.", promptStyles.Length);

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
}
