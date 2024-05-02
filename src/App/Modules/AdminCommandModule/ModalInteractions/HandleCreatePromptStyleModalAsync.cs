using Discord;
using Discord.Interactions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MuzakBot.Lib.Models.CommandModules;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.App.Modules;

public partial class AdminCommandModule
{
    [ModalInteraction("prompt-style-create")]
    public async Task HandleCreatePromptStyleModalAsync(LyricsAnalyzerPromptStyleModal promptStyleModal)
    {
        await Context.Interaction.DeferAsync(
            ephemeral: true
        );

        using var dbContext = _lyricsAnalyzerDbContextFactory.CreateDbContext();

        LyricsAnalyzerPromptStyle promptStyle = new(promptStyleModal);

        EmbedBuilder embed;

        bool promptStyleExists = dbContext.LyricsAnalyzerPromptStyles
            .WithPartitionKey("prompt-style")
            .ToList()
            .Any(item => item.ShortName == promptStyle.ShortName);

        if (promptStyleExists)
        {
            embed = new EmbedBuilder()
                .WithTitle("Lyrics Analyzer Prompt Style")
                .WithDescription($"A prompt style with the short name `{promptStyle.ShortName}` already exists.")
                .WithColor(Color.Red);

            await Context.Interaction.FollowupAsync(
                embeds: [
                    embed.Build()
                    ]
            );

            return;
        }

        try
        {
            dbContext.LyricsAnalyzerPromptStyles.Add(promptStyle);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            embed = new EmbedBuilder()
                .WithTitle("Lyrics Analyzer Prompt Style")
                .WithDescription($"An error occurred while creating the prompt style:\n\n{ex.Message}")
                .WithColor(Color.Red);

            await Context.Interaction.FollowupAsync(
                embeds: [
                    embed.Build()
                    ]
            );

            _logger.LogError(ex, "An error occurred while creating the prompt style: {Message}", ex.Message);

            return;
        }

        embed = new EmbedBuilder()
            .WithTitle("Lyrics Analyzer Prompt Style")
            .WithDescription($"Successfully created prompt style **{promptStyle.Name}**.")
            .WithColor(Color.Green);

        await Context.Interaction.FollowupAsync(
            embeds: [
                embed.Build()
                ]
        );
    }
}