using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Services;

namespace MuzakBot.App;

public class AppleMusicArtistSongAutocompleteHandler : AutocompleteHandler
{
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly ILogger _logger;

    public AppleMusicArtistSongAutocompleteHandler(IAppleMusicApiService appleMusicApiService, ILogger<AppleMusicArtistSongAutocompleteHandler> logger)
    {
        _appleMusicApiService = appleMusicApiService;
        _logger = logger;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = [];

        AutocompleteOption? artistInput = autocompleteInteraction.Data.Options.FirstOrDefault(item => item.Name == "artist-name");

        Artist artist;
        if (artistInput is null || artistInput.Value is null || string.IsNullOrWhiteSpace(artistInput.Value.ToString()))
        {
            return AutocompletionResult.FromError(
                error: InteractionCommandError.BadArgs,
                reason: "Please select an artist first. 😅"
            );
        }

        artist = await _appleMusicApiService.GetArtistFromCatalogAsync(artistInput.Value.ToString()!);

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

        Song[] songSearchResults;
        try
        {
            songSearchResults = await _appleMusicApiService.SearchSongsAsync($"{artist.Attributes!.Name} {autocompleteInteraction.Data?.Current?.Value?.ToString()!}");
        }
        catch
        {
            return AutocompletionResult.FromError(
                error: InteractionCommandError.Unsuccessful,
                reason: "An error occurred while searching for the song. 😥"
            );
        }

        if (songSearchResults.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: "No results found"
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        Song[] filteredSongs = Array.FindAll(
            array: songSearchResults,
            match: song => song.Attributes!.ArtistName == artist.Attributes!.Name || song.Attributes!.ArtistName.Contains(artist.Attributes!.Name)
        );

        if (filteredSongs.Length == 0)
        {
            return AutocompletionResult.FromError(
                error: InteractionCommandError.Unsuccessful,
                reason: "No songs found for the selected artist. 😅"
            );
        }

        foreach (var songItem in filteredSongs)
        {
            _logger.LogInformation("Song: {SongName} - {SongId}", songItem.Attributes!.Name, songItem.Id);
            results.Add(
                item: new(
                    name: songItem.Attributes!.Name,
                    value: songItem.Id
                )
            );
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}
