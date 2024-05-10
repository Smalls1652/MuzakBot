using Discord;
using Discord.Interactions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.Itunes;
using MuzakBot.Lib.Models.MusicBrainz;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Handlers;

public class ArtistSongNameSearchAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<ArtistSongNameSearchAutoCompleteHandler> _logger;
    private readonly IMusicBrainzService _musicBrainzService;

    public ArtistSongNameSearchAutoCompleteHandler(ILogger<ArtistSongNameSearchAutoCompleteHandler> logger, IMusicBrainzService musicBrainzService)
    {
        _logger = logger;
        _musicBrainzService = musicBrainzService;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = new();

        AutocompleteOption? artistInput = autocompleteInteraction.Data.Options.FirstOrDefault(item => item.Name == "artist-name");

        if (artistInput is null || artistInput.Value is null || string.IsNullOrWhiteSpace(artistInput.Value.ToString()))
        {
            results.Add(
                item: new(
                    name: "⚠️ Select an artist first",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        if (autocompleteInteraction.Data is null || autocompleteInteraction.Data.Current is null || autocompleteInteraction.Data.Current.Value is null || string.IsNullOrWhiteSpace(autocompleteInteraction.Data.Current.Value.ToString()))
        {
            results.Add(
                item: new(
                    name: "Start typing a song name...",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        MusicBrainzRecordingSearchResult? songSearchResult = await _musicBrainzService.SearchArtistByNameRecordingsAsync(artistInput.Value.ToString()!, autocompleteInteraction.Data?.Current?.Value?.ToString()!);

        if (songSearchResult is null || songSearchResult.Recordings is null || songSearchResult.Recordings.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        MusicBrainzRecordingItem[]? distinctSongItems = songSearchResult.GetDistinct();

        if (distinctSongItems is null || distinctSongItems.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        foreach (MusicBrainzRecordingItem songItem in distinctSongItems)
        {
            results.Add(
                item: new(
                    name: songItem.Title,
                    value: songItem.Title
                )
            );
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}