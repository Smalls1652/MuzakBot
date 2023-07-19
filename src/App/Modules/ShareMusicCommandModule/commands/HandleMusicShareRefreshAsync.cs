using Discord;
using Discord.Interactions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{

    [ComponentInteraction(customId: "refresh-musiclinks-btn-*")]
    private async Task HandleMusicShareRefreshAsync(string url)
    {
        await Context.Interaction.DeferAsync(
            ephemeral: false
        );

        _logger.LogInformation("Refreshing music share links for {url}", url);

        // Get the music entity item from Odesli.
        MusicEntityItem? musicEntityItem = await _odesliService.GetShareLinksAsync(url);

        // Generate the music share components.
        ComponentBuilder linksComponentBuilder = GenerateMusicShareComponent(musicEntityItem!);

        // Modify the message with the updated components.
        await Context.Interaction.ModifyOriginalResponseAsync(
            messageProps => messageProps.Components = linksComponentBuilder.Build()
        );

        _logger.LogInformation("Refreshed music share links for {url}", url);
    }
}