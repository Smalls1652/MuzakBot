using System.Text;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.App.Handlers;
using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.App.Modules;

public partial class AlbumReleaseLookupCommandModule
{
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [SlashCommand(
        name: "lookup",
        description: "Look up the release date of an album."
    )]
    private async Task AlbumReleaseLookupSlashCommandAsync(
        [Summary("artistName", "The name of an artist."),
         Autocomplete(typeof(AppleMusicArtistAutoCompleteHandler))
        ]
        string artistId,
        [Summary("albumName", "The name of an album."),
         Autocomplete(typeof(AppleMusicArtistAlbumAutocompleteHandler))
        ]
        string albumId
    )
    {
        await DeferAsync();

        Album album;
        try
        {
            album = await _appleMusicApiService.GetAlbumFromCatalogAsync(albumId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get album from Apple Music API.");
            
            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the album. 😥").Build(),
                components: GenerateRemoveComponent().Build()
            );

            return;
        }

        if (album.Attributes!.IsComplete)
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("This album has already been released.").Build(),
                components: GenerateRemoveComponent().Build()
            );
            return;
        }

        DateOnly albumReleaseDate = album.Attributes!.ReleaseDate;
        DateTimeOffset albumReleaseDateTime = new(albumReleaseDate.Year, albumReleaseDate.Month, albumReleaseDate.Day, 12, 0, 0, TimeSpan.Zero);

        StringBuilder descriptionBuilder = new($"by {album.Attributes!.ArtistName}\n\n");

        descriptionBuilder.AppendLine($"Releases <t:{albumReleaseDateTime.ToUnixTimeSeconds()}:R> (approximately).");

        string artworkUrl = album.Attributes!.Artwork.Url
            .Replace(
                oldValue: "{w}",
                newValue: "512"
            )
            .Replace(
                oldValue: "{h}",
                newValue: "512"
            );

        EmbedBuilder embed = new EmbedBuilder()
            .WithTitle(album.Attributes!.Name)
            .WithDescription(descriptionBuilder.ToString())
            .WithColor(Color.Green)
            .WithImageUrl(artworkUrl);

        await FollowupAsync(
            embed: embed.Build()
        );
    }
}