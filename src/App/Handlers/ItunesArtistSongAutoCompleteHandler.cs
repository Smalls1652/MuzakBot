using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class ItunesArtistSongAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<ItunesArtistSongAutoCompleteHandler> _logger;
    private readonly IItunesApiService _itunesApiService;

    public ItunesArtistSongAutoCompleteHandler(ILogger<ItunesArtistSongAutoCompleteHandler> logger, IItunesApiService itunesApiService)
    {
        _logger = logger;
        _itunesApiService = itunesApiService;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        AutocompleteOption? artistInput = autocompleteInteraction.Data.Options.FirstOrDefault(item => item.Name == "artist-name");

        if (artistInput is null || artistInput.Value is null || string.IsNullOrWhiteSpace(artistInput.Value.ToString()))
        {
            results.Add(
                item: new(
                    name: "⚠️ Enter an artist name first",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        if ( autocompleteInteraction.Data is null || autocompleteInteraction.Data.Current is null || autocompleteInteraction.Data.Current.Value is null || string.IsNullOrWhiteSpace(autocompleteInteraction.Data.Current.Value.ToString()))
        {
            results.Add(
                item: new(
                    name: "Start typing a song name...",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        ApiSearchResult<ArtistItem>? artistSearchResult = await _itunesApiService.GetArtistIdLookupResultAsync(artistInput.Value.ToString()!);

        ApiSearchResult<SongItem>? songSearchResult = await _itunesApiService.GetSongsByArtistResultAsync(artistSearchResult.Results[0], autocompleteInteraction.Data?.Current?.Value?.ToString()!);

        if (songSearchResult is null || songSearchResult.Results is null || songSearchResult.Results.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }
        else
        {
            foreach (SongItem songItem in songSearchResult.Results)
            {
                results.Add(
                    item: new(
                        name: songItem.TrackName,
                        value: songItem.TrackId.ToString()
                    )
                );
            }

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }
    }
}