using System.Text;

using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.App.Handlers;
using MuzakBot.App.Models.Responses;
using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class AlbumReleaseCommandModule
{
    /// <summary>
    /// Handles the 'lookup' slash command.
    /// </summary>
    /// <param name="artistId">The ID of an artist.</param>
    /// <param name="albumId">The ID of an album.</param>
    /// <returns></returns>
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

        MusicEntityItem musicEntityItem;
        try
        {
            musicEntityItem = await _odesliService.GetShareLinksAsync(album.Attributes!.Url)
                ?? throw new Exception("Failed to get share links from Odesli API.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get share links from Odesli API.");

            await FollowupAsync(
                embed: GenerateErrorEmbed("An error occurred while getting the share links. 😥").Build(),
                components: GenerateRemoveComponent().Build()
            );

            return;
        }

        using AlbumReleaseLookupResponse albumReleaseLookupResponse = new(album, musicEntityItem);

        await FollowupWithFileAsync(
            embed: albumReleaseLookupResponse.GenerateEmbed().Build(),
            components: albumReleaseLookupResponse.GenerateComponent().Build(),
            fileName: albumReleaseLookupResponse.AlbumArtFileName,
            fileStream: albumReleaseLookupResponse.AlbumArtworkStream
        );
    }
}