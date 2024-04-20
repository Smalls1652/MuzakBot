using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Extensions;
using MuzakBot.App.Services;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Message command for parsing a message and getting the share links for any music links found in the message.
    /// </summary>
    /// <param name="message">The message selected by the client.</param>
    /// <exception cref="Exception"></exception>
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [MessageCommand(
        name: "Get music share links"
    )]
    private async Task GetMusicShareLinksFromPostMessageCommandAsync(IMessage message)
    {
        using var activity = _activitySource.StartGetMusicShareLinksFromPostMessageCommandAsyncActivity(message, Context);

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

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            MatchCollection linkMatches = linkRegex.Matches(message.CleanContent);

            bool hasMultipleLinks = linkMatches.Count > 1;

            string url = linkMatches[0].Groups["musicLink"].Value;

            MusicEntityItem musicEntityItem;
            try
            {
                musicEntityItem = await GetMusicEntityItemAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No share links found for '{url}'.", url);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("No share links were found. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            PlatformEntityLink platformEntityLink;
            try
            {
                platformEntityLink = GetPlatformEntityLink(musicEntityItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not get all of the necessary data for '{url}'.", url);
                await FollowupAsync(
                    embed: GenerateErrorEmbed("I was unable to get the necessary information from Odesli. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                return;
            }

            StreamingEntityItem streamingEntityItem = musicEntityItem.EntitiesByUniqueId![platformEntityLink.EntityUniqueId!];

            Stream albumArtStream;
            try
            {
                albumArtStream = await GetAlbumArtStreamAsync(streamingEntityItem);
            }
            catch (Exception)
            {
                await FollowupAsync(
                    embed: GenerateErrorEmbed("I ran into an issue while retrieving the album artwork. 😥").Build(),
                    components: GenerateRemoveComponent().Build()
                );

                activity?.SetStatus(ActivityStatusCode.Error);

                throw;
            }

            var linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem);

            var messageEmbed = GenerateEmbedBuilder(streamingEntityItem);

            StringBuilder messageBuilder = new();

            if (hasMultipleLinks)
            {
                messageBuilder.AppendLine("> ⚠️ **Warning:**");
                messageBuilder.AppendLine("> There were multiple links found. Only showing the first one. 😰");
            }

            await FollowupWithFileAsync(
                text: messageBuilder.ToString(),
                embed: messageEmbed.Build(),
                fileStream: albumArtStream,
                fileName: $"{streamingEntityItem.Id}.jpg",
                components: linksComponentBuilder.Build(),
                ephemeral: true
            );

            albumArtStream.Dispose();
        }
        finally
        {
            _commandMetrics.IncrementGetLinksFromPostCounter();
        }
    }
}