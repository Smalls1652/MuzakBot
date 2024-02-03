using Discord;
using Discord.Interactions;
using Microsoft.Extensions.Logging;
using MuzakBot.App.Handlers;
using MuzakBot.Lib.Models.CommandModules;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    [ModalInteraction("prompt-style-edit-user-prompt")]
    public async Task HandleEditUserPromptStyleModalAsync(LyricsAnalyzerPromptStyleUserPromptModal userPromptModal)
    {
        await Context.Interaction.DeferAsync(
            ephemeral: true
        );

        EmbedBuilder embed;

        LyricsAnalyzerPromptStyle? promptStyle = await _cosmosDbService.GetLyricsAnalyzerPromptStyleAsync(userPromptModal.ShortName);

        if (promptStyle is null)
        {
            embed = new EmbedBuilder()
                .WithTitle("Lyrics Analyzer Prompt Style")
                .WithDescription("The prompt style short name appears to have been changed.\n\n**Prompt style short names cannot be changed!**")
                .WithColor(Color.Red);

            await Context.Interaction.FollowupAsync(
                embeds: [
                    embed.Build()
                    ]
            );

            return;
        }

        promptStyle.UserPrompt = userPromptModal.UserPrompt;

        promptStyle.UpdateLastUpdateTime();

        try
        {
            await _cosmosDbService.AddOrUpdateLyricsAnalyzerPromptStyleAsync(promptStyle);
        }
        catch (Exception ex)
        {
            embed = new EmbedBuilder()
                .WithTitle("Lyrics Analyzer Prompt Style")
                .WithDescription($"An error occurred while updating the prompt style:\n\n{ex.Message}")
                .WithColor(Color.Red);

            await Context.Interaction.FollowupAsync(
                embeds: [
                    embed.Build()
                    ]
            );

            _logger.LogError(ex, "An error occurred while updating the prompt style: {Message}", ex.Message);

            return;
        }

        embed = new EmbedBuilder()
            .WithTitle("Lyrics Analyzer Prompt Style")
            .WithDescription($"Successfully updated prompt style **{promptStyle.Name}**.")
            .WithColor(Color.Green);

        await Context.Interaction.FollowupAsync(
            embeds: [
                embed.Build()
                ]
        );
    }
}