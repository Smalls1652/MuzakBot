using Discord;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Services;

namespace MuzakBot.App.Handlers;

public class ArtistAlbumSearchAutoCompleteHandler : AutocompleteHandler
{
    private readonly ILogger<ArtistAlbumSearchAutoCompleteHandler> _logger;
    private readonly IMusicBrainzService _musicBrainzService;

    public ArtistAlbumSearchAutoCompleteHandler(ILogger<ArtistAlbumSearchAutoCompleteHandler> logger, IMusicBrainzService musicBrainzService)
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
                    name: "Start typing an album name...",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        MusicBrainzReleaseSearchResult? albumSearchResult = await _musicBrainzService.SearchArtistReleasesAsync(artistInput.Value.ToString()!, autocompleteInteraction.Data?.Current?.Value?.ToString()!);

        if (albumSearchResult is null || albumSearchResult.Releases is null || albumSearchResult.Releases.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        MusicBrainzReleaseItem[]? distinctAlbumItems = albumSearchResult.GetDistinct();

        if (distinctAlbumItems is null || distinctAlbumItems.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: ""
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        foreach (MusicBrainzReleaseItem releaseItem in distinctAlbumItems)
        {
            results.Add(
                item: new(
                    name: releaseItem.Title,
                    value: releaseItem.Id
                )
            );
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}