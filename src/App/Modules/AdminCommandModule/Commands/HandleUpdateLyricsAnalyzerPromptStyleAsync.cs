using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Handlers;
using MuzakBot.Lib.Models.CommandModules;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    [SlashCommand(name: "update-prompt-style", description: "Update a lyrics analyzer prompt style.")]
    [RequireOwner]
    public async Task HandleUpdateLyricsAnalyzerPromptStyleAsync(
        [Summary(name: "style", description: "The prompt style to edit."),
        Autocomplete(typeof(LyricsAnalyzerPromptStyleAutoCompleteHandler))]
        string style,
        [Summary(name: "configType", description: "The configuration type to edit."),
        Choice(name: "Core", value: "core"),
        Choice(name: "User prompt", value: "user-prompt")]
        string configType = "core"
    )
    {
        LyricsAnalyzerPromptStyle? promptStyle = await _cosmosDbService.GetLyricsAnalyzerPromptStyleAsync(style);

        if (promptStyle is null)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("Lyrics Analyzer Prompt Style")
                .WithDescription($"A prompt style with the short name `{style}` does not exist.")
                .WithColor(Color.Red);

            await Context.Interaction.FollowupAsync(
                embeds: [
                    embed.Build()
                    ]
            );

            return;
        }

        if (configType == "core")
        {
            LyricsAnalyzerPromptStyleModal promptStyleModal = new(promptStyle);

            await RespondWithModalAsync(
                customId: $"prompt-style-edit",
                modal: promptStyleModal
            );

            return;
        }

        if (configType == "user-prompt")
        {
            LyricsAnalyzerPromptStyleUserPromptModal promptStyleUserPromptModal = new(promptStyle);
            await RespondWithModalAsync(
                customId: $"prompt-style-edit-user-prompt",
                modal: promptStyleUserPromptModal
            );

            return;
        }
    }
}