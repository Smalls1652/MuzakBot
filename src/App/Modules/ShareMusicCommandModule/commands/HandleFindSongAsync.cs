using System.Text;
using System.Text.Json;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Handlers;
using MuzakBot.App.Models.Itunes;
using MuzakBot.App.Models.MusicBrainz;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    [SlashCommand(
        name: "findsong",
        description: "Find music from an artist"
    )]
    private async Task HandleFindSongAsync(
        [Summary("artistName", "The name of an artist"),
         Autocomplete(typeof(ArtistSearchAutoCompleteHandler))
        ]
        string artistId,
        [Summary(name: "songName", description: "The name of a song"),
         Autocomplete(typeof(ArtistSongSearchAutoCompleteHandler))
        ]
        string songId
    )
    {
        await DeferAsync(
            ephemeral: false
        );

        MusicBrainzArtistItem? artistItem = await _musicBrainzService.LookupArtistAsync(artistId);

        MusicBrainzRecordingItem? recordingItem = await _musicBrainzService.LookupRecordingAsync(songId);

        MusicBrainzReleaseItem? releaseItem = null;

        ApiSearchResult<SongItem>? apiSearchResult = await _itunesApiService.GetSongsByArtistResultAsync(artistItem!.Name, recordingItem!.Title);

        if (apiSearchResult is null || apiSearchResult.Results is null || apiSearchResult.Results.Length == 0)
        {
            await FollowupAsync(
                text: "No results found",
                ephemeral: false
            );

            return;
        }


        SongItem songItem = apiSearchResult.Results[0];

        MusicEntityItem? musicEntityItem = null;
        try
        {
            musicEntityItem = await _odesliService.GetShareLinksAsync(songItem.TrackViewUrl);

            if (musicEntityItem is null)
            {
                throw new Exception("No share links found.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "No share links found for '{url}'.", songItem.TrackViewUrl);
            await FollowupAsync(
                text: "No share links were found for that URL. 😥",
                components: GenerateRemoveComponent().Build()
            );

            return;
        }

        PlatformEntityLink? platformEntityLink;
        try
        {
            platformEntityLink = musicEntityItem.LinksByPlatform!["itunes"];
        }
        catch
        {
            var streamingEntityWithThumbnailUrl = musicEntityItem.EntitiesByUniqueId!.FirstOrDefault(entity => entity.Value.ThumbnailUrl is not null).Value.ApiProvider;

            if (!string.IsNullOrEmpty(streamingEntityWithThumbnailUrl))
            {
                platformEntityLink = musicEntityItem.LinksByPlatform![streamingEntityWithThumbnailUrl];
            }
            else
            {
                _logger.LogError("Could get all of the necessary data for '{url}'.", songItem.TrackViewUrl);
                await FollowupAsync(
                    text: "I was unable to get the necessary information from Odesli. 😥",
                    components: GenerateRemoveComponent().Build()
                );

                return;
            }
        }

        StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];
        using var albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);

        var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

        string uniqueFileName = Guid.NewGuid().ToString();

        var messageEmbed = new EmbedBuilder()
            .WithTitle(streamingEntityItem.Title)
            .WithDescription($"by {streamingEntityItem.ArtistName}")
            .WithColor(Color.DarkBlue)
            .WithImageUrl($"attachment://{uniqueFileName}.jpg")
            .WithFooter("(Powered by Songlink/Odesli)");

        await FollowupWithFileAsync(
            embed: messageEmbed.Build(),
            fileStream: albumArtStream,
            fileName: $"{uniqueFileName}.jpg",
            components: linksComponentBuilder.Build()
        );
    }
}