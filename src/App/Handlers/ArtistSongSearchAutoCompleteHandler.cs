using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class ArtistSongSearchAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<ArtistSongSearchAutoCompleteHandler> _logger;
    private readonly IItunesApiService _itunesApiService;
    private readonly IMusicBrainzService _musicBrainzService;

    public ArtistSongSearchAutoCompleteHandler(ILogger<ArtistSongSearchAutoCompleteHandler> logger, IItunesApiService itunesApiService, IMusicBrainzService musicBrainzService)
    {
        _logger = logger;
        _itunesApiService = itunesApiService;
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
                    name: "⚠️ Enter an artist name first",
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

        MusicBrainzRecordingSearchResult? songSearchResult = await _musicBrainzService.SearchArtistRecordingsAsync(artistInput.Value.ToString()!, autocompleteInteraction.Data?.Current?.Value?.ToString()!);

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


        foreach (MusicBrainzRecordingItem songItem in songSearchResult.Recordings)
        {
            if (songItem.Releases is not null && songItem.Releases.Length > 0)
            {
                results.Add(
                    item: new(
                        name: songItem.Title,
                        value: songItem.Id
                    )
                );
            }
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}