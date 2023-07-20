using Discord;
using Discord.Interactions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    [ComponentInteraction(customId: "remove-sharemusic-post-btn-*")]
    private async Task HandleMusicShareMessageRemovalAsync()
    {
        await DeferAsync();

        _logger.LogInformation("Removing music share message '{id}'. Initiated by '{username}'", Context.Interaction.Id.ToString(), Context.User.Username);

        await DeleteOriginalResponseAsync();
    }
}