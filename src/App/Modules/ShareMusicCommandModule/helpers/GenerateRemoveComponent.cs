using Discord;
using MuzakBot.App.Models.Odesli;
using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
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