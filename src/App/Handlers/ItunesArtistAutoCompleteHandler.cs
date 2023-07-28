using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class ItunesArtistAutoCompleteHandler : AutocompleteHandler
{
    private readonly IItunesApiService _itunesApiService;

    public ItunesArtistAutoCompleteHandler(IItunesApiService itunesApiService)
    {
        _itunesApiService = itunesApiService;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        if ( autocompleteInteraction.Data is null || autocompleteInteraction.Data.Current is null || autocompleteInteraction.Data.Current.Value is null || string.IsNullOrWhiteSpace(autocompleteInteraction.Data.Current.Value.ToString()))
        {
            results.Add(
                item: new(
                    name: "Start typing an artist name...",
                    value: "Start typing an artist name..."
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        ApiSearchResult<ArtistItem>? artistSearchResult = await _itunesApiService.GetArtistSearchResultAsync(autocompleteInteraction.Data?.Current?.Value?.ToString()!);

        if (artistSearchResult is null || artistSearchResult.Results is null || artistSearchResult.Results.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: "No results found"
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }
        else
        {
            foreach (ArtistItem artistItem in artistSearchResult.Results)
            {
                results.Add(
                    item: new(
                        name: artistItem.ArtistName,
                        value: artistItem.ArtistId.ToString()
                    )
                );
            }

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }
    }
}