using Discord;
using Discord.Interactions;
using MuzakBot.App.Extensions;
using MuzakBot.Lib.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Models.Responses;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Refreshes the share links on a message.
    /// </summary>
    /// <remarks>
    /// This is a component interaction handler and can only be
    /// invoked by a user clicking the "Refresh" button on an
    /// existing message.
    /// </remarks>
    /// <param name="url">The Songlink/Odesli URL.</param>
    [ComponentInteraction(customId: "refresh-musiclinks-btn-*")]
    private async Task HandleMusicShareRefreshAsync(string url)
    {
        using var activity = _activitySource.StartHandleMusicShareRefreshAsyncActivity(url, Context);
        
        await Context.Interaction.DeferAsync(
            ephemeral: false
        );

        _logger.LogInformation("Refreshing music share links for {url}", url);

        // Get the music entity item from Odesli.
        MusicEntityItem? musicEntityItem = await _odesliService.GetShareLinksAsync(url, activity?.Id);

        ShareMusicResponse shareMusicResponse = new(musicEntityItem!);

        // Modify the message with the updated components.
        await Context.Interaction.ModifyOriginalResponseAsync(
            messageProps => messageProps.Components = shareMusicResponse.GenerateComponent().Build()
        );

        _logger.LogInformation("Refreshed music share links for {url}", url);
    }
}