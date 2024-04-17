using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Configuration;

using MuzakBot.App.Models.Responses;

namespace MuzakBot.App.Modules;

public partial class CoreCommandModule
{
    /// <summary>
    /// Handles the 'muzakbot-info' slash command.
    /// </summary>
    /// <returns></returns>
    [CommandContextType(InteractionContextType.BotDm, InteractionContextType.PrivateChannel, InteractionContextType.Guild)]
    [IntegrationType(ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
    [SlashCommand(
        name: "muzakbot-info",
        description: "Show helpful information about MuzakBot."
    )]
    private async Task HandleBotInfoCommandAsync()
    {
        await DeferAsync(
            ephemeral: true
        );

        BotInfoResponse botInfoResponse = new(
            botInviteUrl: _configuration.GetValue<string>("BOT_INVITE_URL")
        );

        await FollowupAsync(
            embed: botInfoResponse.GenerateEmbed().Build(),
            components: botInfoResponse.GenerateComponent().Build()
        );
    }
}
