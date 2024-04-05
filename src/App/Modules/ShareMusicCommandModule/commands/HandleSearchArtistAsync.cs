using System.Text;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.App.Handlers;
using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [SlashCommand(
        name: "searchartist",
        description: "Find an album from an artist"
    )]
    private async Task HandleSearchArtistAsync(
        [Summary(
            name: "searchTerm",
            description: "The search term for the search."
        ),
        Autocomplete(typeof(AppleMusicArtistAutoCompleteHandler))]
        string searchTerm
    )
    {
        await DeferAsync();

        Artist artist;

        try
        {
            artist = await _appleMusicApiService.GetArtistFromCatalogAsync(searchTerm);
        }
        catch
        {
            await FollowupAsync(
                text: "An error occurred while searching for the artist. 😥"
            );

            return;
        }

        StringBuilder outputResponseBuilder = new($"# {artist.Attributes!.Name}\n");

        outputResponseBuilder.AppendLine("## Genres\n");
        if (artist.Attributes!.GenreNames is not null && artist.Attributes!.GenreNames.Length > 0)
        {
            foreach (string genre in artist.Attributes!.GenreNames)
            {
                outputResponseBuilder.AppendLine($"- {genre}");
            }
        }
        else
        {
            outputResponseBuilder.AppendLine("- No genres found.");
        }

        if (artist.Attributes!.Artwork is null)
        {
            await FollowupAsync(
                text: outputResponseBuilder.ToString()
            );

            return;
        }
        else
        {
            var httpClient = _httpClientFactory.CreateClient("GenericClient");

            string artworkUrl = artist.Attributes!.Artwork
                .Url
                .Replace("{w}", (artist.Attributes!.Artwork.Width / 2).ToString())
                .Replace("{h}", (artist.Attributes!.Artwork.Height / 2).ToString());

            _logger.LogInformation("Getting {artworkUrl}.", artworkUrl);

            HttpResponseMessage responseMessage = await httpClient.GetAsync(artworkUrl);

            var embedBuilder = new EmbedBuilder()
                .WithImageUrl($"attachment://{artist.Id}.jpg")
                .WithColor(Color.DarkBlue);

            await FollowupWithFileAsync(
                text: outputResponseBuilder.ToString(),
                fileStream: await responseMessage.Content.ReadAsStreamAsync(),
                fileName: $"{artist.Id}.jpg",
                embed: embedBuilder.Build()
            );

            return;
        }
    }
}
