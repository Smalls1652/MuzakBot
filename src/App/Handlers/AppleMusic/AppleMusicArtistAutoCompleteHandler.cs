using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Services;

namespace MuzakBot.App.Handlers;

public class AppleMusicArtistAutoCompleteHandler : AutocompleteHandler
{
    private readonly IAppleMusicApiService _appleMusicApiService;
    private readonly ILogger _logger;

    public AppleMusicArtistAutoCompleteHandler(IAppleMusicApiService appleMusicApiService, ILogger<AppleMusicArtistAutoCompleteHandler> logger)
    {
        _appleMusicApiService = appleMusicApiService;
        _logger = logger;
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        List<AutocompleteResult> results = [];

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

        Artist[] artistSearchResults;
        try
        {
            artistSearchResults = await _appleMusicApiService.SearchArtistsAsync(autocompleteInteraction.Data?.Current?.Value?.ToString()!);
        }
        catch
        {
            return AutocompletionResult.FromError(
                error: InteractionCommandError.Unsuccessful,
                reason: "An error occurred while searching for artists. 😥"
            );
        }

        if (artistSearchResults.Length == 0)
        {
            results.Add(
                item: new(
                    name: "No results found",
                    value: "No results found"
                )
            );

            return AutocompletionResult.FromSuccess(results.AsEnumerable());
        }

        foreach (var artistItem in artistSearchResults)
        {
            string primaryGenre;

            try
            {
                primaryGenre = artistItem.Attributes!.GenreNames.First();
            }
            catch
            {
                primaryGenre = "Unknown";
            }

            results.Add(
                item: new(
                    name: $"{artistItem.Attributes!.Name} ({primaryGenre})",
                    value: artistItem.Id
                )
            );
        }

        return AutocompletionResult.FromSuccess(results.AsEnumerable());
    }
}
