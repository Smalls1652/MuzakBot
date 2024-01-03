using System.Text;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Message command for parsing a message and getting the share links for any music links found in the message.
    /// </summary>
    /// <param name="message">The message selected by the client.</param>
    /// <exception cref="Exception"></exception>
    [MessageCommand(
        name: "Get music share links"
    )]
    private async Task HandleGetLinksFromPostAsync(IMessage message)
    {
        using var activity = _activitySource.StartActivity(
            name: "HandleGetLinksFromPostAsync",
            kind: ActivityKind.Server,
            tags: new ActivityTagsCollection
            {
                { "message_Id", message.Id },
                { "command_Type", "MessageCommand"},
                { "command_Name", "Get music share links" },
                { "guild_Id", Context.Guild.Id },
                { "guild_Name", Context.Guild.Name },
                { "channel_Id", Context.Channel.Id },
                { "channel_Name", Context.Channel.Name }
            }
        );
        
        try
        {
            Regex linkRegex = new(@"(?'musicLink'(?>https|http):\/\/(?>[A-Za-z0-9\.]+)(?>\/\S*[^\.\s]|))(?> |)", RegexOptions.Multiline);

            await DeferAsync(
                ephemeral: true
            );

            _logger.LogInformation("Message content: {messageContent}", message.CleanContent);

            if (!linkRegex.IsMatch(message.CleanContent))
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("Could not find any links in that post. 🤔").Build(),
                    ephemeral: true
                );

                return;
            }

            MatchCollection linkMatches = linkRegex.Matches(message.CleanContent);

            StringBuilder stringBuilder = new("Results:\n");
            for (var i = 0; i < linkMatches.Count; i++)
            {
                string url = linkMatches[i].Groups["musicLink"].Value;

                MusicEntityItem? musicEntityItem = null;
                try
                {
                    musicEntityItem = await _odesliService.GetShareLinksAsync(url);

                    if (musicEntityItem is null)
                    {
                        throw new Exception("No share links found.");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "No share links found for '{url}'.", url);
                    stringBuilder.AppendLine($"{i + 1}. No share links were found for `{url}`.");
                    continue;
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
                        _logger.LogError("Couldn't get all of the necessary data for '{url}'.", url);
                        stringBuilder.AppendLine($"{i + 1}. Couldn't get all of the necessary data from Odesli for `{url}`.");
                        continue;
                    }
                }

                StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];
                using var albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);

                var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

                var messageEmbed = GenerateEmbedBuilder(streamingEntityItem);

                await Context.Channel.SendFileAsync(
                    embed: messageEmbed.Build(),
                    stream: albumArtStream,
                    filename: $"{streamingEntityItem.Id}.jpg",
                    components: linksComponentBuilder.Build(),
                    messageReference: new(message.Id),
                    allowedMentions: AllowedMentions.None
                );

                stringBuilder.AppendLine($"{i + 1}. Successfully got links for `{url}`.");
            }

            await FollowupAsync(
                text: stringBuilder.ToString(),
                ephemeral: true
            );
        }
        finally
        {
            _commandMetrics.IncrementGetLinksFromPostCounter();
        }
    }
}