using Discord;

using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Generates a <see cref="ComponentBuilder"/> for the "Remove message"
    /// button on a failed music share message.
    /// </summary>
    /// <returns></returns>
    private ComponentBuilder GenerateRemoveComponent()
    {
        Guid guid = Guid.NewGuid();
        // Create the remove button component.
        ButtonBuilder removeButton = new(
            label: "Remove message",
            style: ButtonStyle.Danger,
            emote: new Emoji("🗑️"),
            customId: $"remove-sharemusic-post-btn-{guid}"
        );

        return new ComponentBuilder()
            .WithButton(removeButton);
    }
}
