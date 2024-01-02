using System.Diagnostics;
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
        using var activity = _activitySource.StartActivity(
            name: "HandleFindSongAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "command_Type", "SlashCommand"},
                { "command_Name", "findsong" },
                { "artist_Id", artistId },
                { "song_Id", songId },
                { "user_Id", Context.User.Id },
                { "user_Username", Context.User.Username },
                { "guild_Id", Context.Guild.Id },
                { "guild_Name", Context.Guild.Name },
                { "channel_Id", Context.Channel.Id },
                { "channel_Name", Context.Channel.Name }
            }
        );
        
        try
        {
            await DeferAsync(
                ephemeral: false
            );

            MusicBrainzArtistItem? artistItem = null;
            try
            {
                artistItem = await _musicBrainzService.LookupArtistAsync(artistId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up artist '{artistId}'.", artistId);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while looking up the artist. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                return;
            }

            MusicBrainzRecordingItem? recordingItem = null;

            try
            {
                recordingItem = await _musicBrainzService.LookupRecordingAsync(songId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up song '{songId}'.", songId);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("An error occurred while looking up the song. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                return;
            }

            if (artistItem is null || recordingItem is null)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No results found for that artist and song. 😥").Build(),
                    components: GenerateRemoveComponent().Build(),
                    ephemeral: false
                );

                return;
            }

            ApiSearchResult<SongItem>? apiSearchResult = await _itunesApiService.GetSongsByArtistResultAsync(artistItem!.Name, recordingItem!.Title);

            if (apiSearchResult is null || apiSearchResult.Results is null || apiSearchResult.Results.Length == 0)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No results were found.").Build(),
                    components: GenerateRemoveComponent().Build(),
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
                    embed: GenerateErrorEmbed("No share links were found. 😥").Build(),
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
                        embed: GenerateErrorEmbed("I was unable to get the necessary information from Odesli. 😥").Build(),
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
        finally
        {
            _commandMetrics.IncrementFindSongCounter();
        }
    }
}