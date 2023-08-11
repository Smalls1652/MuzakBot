using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class ArtistSearchAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<ArtistSearchAutoCompleteHandler> _logger;
    private readonly IMusicBrainzService _musicBrainzService;

    public ArtistSearchAutoCompleteHandler(ILogger<ArtistSearchAutoCompleteHandler> logger, IMusicBrainzService musicBrainzService)
    {
        _logger = logger;
        _musicBrainzService = musicBrainzService;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        if (autocompleteInteraction.Data is null || autocompleteInteraction.Data.Current is null || autocompleteInteraction.Data.Current.Value is null || string.IsNullOrWhiteSpace(autocompleteInteraction.Data.Current.Value.ToString()))
        {
            results.Add(
                item: new(
                    name: "Start typing an artist name...",
                    value: "Start typing an artist name..."
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        MusicBrainzArtistSearchResult? artistSearchResult = await _musicBrainzService.SearchArtistAsync(autocompleteInteraction.Data?.Current?.Value?.ToString()!);

        if (artistSearchResult is null || artistSearchResult.Artists is null || artistSearchResult.Artists.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: "No results found"
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        foreach (MusicBrainzArtistItem artistItem in artistSearchResult.Artists)
        {
            results.Add(
                item: new(
                    name: artistItem.Name,
                    value: artistItem.Id
                )
            );
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}