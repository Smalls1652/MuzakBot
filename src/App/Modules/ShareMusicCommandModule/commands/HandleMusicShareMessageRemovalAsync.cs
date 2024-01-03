using Discord;
using Discord.Interactions;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.Odesli;
using MuzakBot.App.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Handler for removing a failed share message.
    /// </summary>
    /// <remarks>
    /// This is a component interaction handler and can only be
    /// invoked by a user clicking the "Remove message" button on
    /// a failed share message.
    /// </remarks>
    [ComponentInteraction(customId: "remove-sharemusic-post-btn-*")]
    private async Task HandleMusicShareMessageRemovalAsync()
    {
        using var activity = _activitySource.StartHandleMusicShareMessageRemovalAsyncActivity(Context);

        await DeferAsync(
            ephemeral: true
        );

        IUserMessage? originalResponse = null;
        try
        {
            originalResponse = await GetOriginalResponseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get original response.");

            await FollowupAsync(
                embed: GenerateErrorEmbed("😵‍💫 Failed to get original response", "Something went wrong while trying to get the original response. Please try again later.").Build(),
                ephemeral: true
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        _logger.LogInformation("Message Id: {MessageId} | User requesting deletion: {requestingUser} | Original user: {originalUser}", originalResponse.Id, Context.User.Id.ToString(), originalResponse.Interaction.User.Id.ToString());
        if (Context.User.Id != originalResponse.Interaction.User.Id)
        {
            await FollowupAsync(
                embed: GenerateErrorEmbed("⛔ Access Denied", "You're not allowed to delete a message not generated by you!").Build(),
                ephemeral: true
            );

            activity?.SetStatus(ActivityStatusCode.Error);

            return;
        }

        _logger.LogInformation("Removing music share message '{id}'. Initiated by '{username}'", originalResponse.Id, Context.User.Username);

        await DeleteOriginalResponseAsync();

        await FollowupAsync(
            text: "Message deleted",
            ephemeral: true
        );
    }
}