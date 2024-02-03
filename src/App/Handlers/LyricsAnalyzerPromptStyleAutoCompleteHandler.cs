using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.Itunes;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class LyricsAnalyzerPromptStyleAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<LyricsAnalyzerPromptStyleAutoCompleteHandler> _logger;
    private readonly ICosmosDbService _cosmosDbService;

    public LyricsAnalyzerPromptStyleAutoCompleteHandler(ILogger<LyricsAnalyzerPromptStyleAutoCompleteHandler> logger, ICosmosDbService cosmosDbService)
    {
        _logger = logger;
        _cosmosDbService = cosmosDbService;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        LyricsAnalyzerPromptStyle[] promptStyles = await _cosmosDbService.GetLyricsAnalyzerPromptStylesAsync();
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