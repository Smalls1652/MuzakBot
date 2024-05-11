using Discord;
using Discord.Interactions;

using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.CommandModules;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    [SlashCommand(name: "create-prompt-style", description: "Create a new prompt style.")]
    [RequireOwner]
    public async Task HandleCreateLyricsAnalyzerPromptStyleAsync()
    {
        try
        {
            await RespondWithModalAsync<LyricsAnalyzerPromptStyleModal>(
                customId: "prompt-style-create"
            );
        }
        catch (Exception ex)
        {
            await RespondAsync(
                embeds: [
                    new EmbedBuilder()
                        .WithTitle("Lyrics Analyzer Prompt Style")
                        .WithDescription($"An error occurred while creating the prompt style:\n\n{ex.Message}")
                        .WithColor(Color.Red)
                        .Build()
                    ]
            );

            _logger.LogError(ex, "An error occurred while creating the prompt style: {Message}", ex.Message);
        }
    }
}
