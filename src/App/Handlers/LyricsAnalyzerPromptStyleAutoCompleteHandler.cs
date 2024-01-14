using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class LyricsAnalyzerPromptStyleAutoCompleteHandler : AutocompleteHandler
{
    private readonly IReadOnlyList<AutocompleteResult> _promptStyleOptions = [
        new("Normal", "normal"),
        new("Meme: Tame", "tame-meme"),
        new("Meme: Insane", "insane-meme"),
        new("Meme: Roast", "roast-meme"),
        new("Snobby Music Critic", "snobby-music-critic")
    ];

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        AutocompleteOption? promptStyleInput = autocompleteInteraction.Data.Options.FirstOrDefault(item => item.Name == "prompt-style");

        if (promptStyleInput is null || promptStyleInput.Value is null || string.IsNullOrWhiteSpace(promptStyleInput.Value.ToString()))
        {
            return AutocompletionResult.FromSuccess(_promptStyleOptions);
        }

        _promptStyleOptions
            .Where(item => item.Value.ToString()!.Contains(promptStyleInput.Value.ToString()!, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(item => results.Add(item));

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}