using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Itunes;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class ArtistNameSearchAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<ArtistNameSearchAutoCompleteHandler> _logger;
    private readonly IMusicBrainzService _musicBrainzService;

    public ArtistNameSearchAutoCompleteHandler(ILogger<ArtistNameSearchAutoCompleteHandler> logger, IMusicBrainzService musicBrainzService)
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

        MusicBrainzArtistItem[] distinctArtists = artistSearchResult.Artists.DistinctBy(artist => artist.Name).ToArray();

        foreach (MusicBrainzArtistItem artistItem in distinctArtists)
        {
            results.Add(
                item: new(
                    name: artistItem.Name,
                    value: artistItem.Name
                )
            );
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}