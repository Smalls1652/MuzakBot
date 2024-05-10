using System.Text;

using Discord;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response for the lyrics analyzer command.
/// </summary>
public class LyricsAnalyzerResponse : IResponse
{
    /// <summary>
    /// Initialize a new instance of the <see cref="LyricsAnalyzerResponse"/> class.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="openAiChatCompletion">The completion from OpenAI's chat API.</param>
    /// <param name="promptStyle">The style of the prompt used for the completion.</param>
    /// <param name="responseId">The response ID.</param>
    /// <param name="analysisId">The ID of the analysis in the database.</param>
    public LyricsAnalyzerResponse(string artistName, string songName, OpenAiChatCompletion openAiChatCompletion, LyricsAnalyzerPromptStyle promptStyle, string responseId, string analysisId)
    {
        ArtistName = artistName;
        SongName = songName;
        ChatCompletion = openAiChatCompletion;
        PromptStyle = promptStyle;
        ResponseId = responseId;
        AnalysisId = analysisId;
    }

    /// <summary>
    /// The name of the artist.
    /// </summary>
    public string ArtistName { get; }

    /// <summary>
    /// The name of the song.
    /// </summary>
    public string SongName { get; }

    /// <summary>
    /// The completion from OpenAI's chat API.
    /// </summary>
    public OpenAiChatCompletion ChatCompletion { get; }

    /// <summary>
    /// The style of the prompt used for the completion.
    /// </summary>
    public LyricsAnalyzerPromptStyle PromptStyle { get; }

    /// <summary>
    /// The response ID.
    /// </summary>
    public string ResponseId { get; }

    /// <summary>
    /// The ID of the analysis in the database.
    /// </summary>
    public string AnalysisId { get; }

    /// <inheritdoc />
    public ComponentBuilder GenerateComponent()
    {
        ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithButton(
                label: "Regenerate",
                style: ButtonStyle.Primary,
                emote: new Emoji("🔄"),
                customId: $"lyrics-analyzer-regenerate-{ResponseId}",
                row: 0
            )
            .WithButton(
                label: "View on website",
                style: ButtonStyle.Link,
                emote: new Emoji("🌐"),
                url: $"https://muzakbot.smalls.online/lyrics-analyzer/analysis/{AnalysisId}"
            );

        return componentBuilder;
    }

    /// <inheritdoc />
    public EmbedBuilder GenerateEmbed()
    {
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ Note")
            .WithDescription(PromptStyle.NoticeText)
            .WithColor(Color.DarkTeal)
            .WithFooter("(Powered by OpenAI's GPT-4)");

        return embedBuilder;
    }

    /// <inheritdoc />
    public string GenerateText()
    {
        StringBuilder lyricsResponseBuilder = new($"# \"{SongName}\" by _{ArtistName}_\n\n");

        lyricsResponseBuilder.AppendLine($"## {PromptStyle.AnalysisType}\n\n");

        string[] analysisLines = ChatCompletion.Choices[0].Message.Content.Split(Environment.NewLine);

        for (int i = 0; i < analysisLines.Length; i++)
        {
            if (i == analysisLines.Length - 1 && string.IsNullOrEmpty(analysisLines[i]))
            {
                break;
            }

            lyricsResponseBuilder.AppendLine($"> {analysisLines[i]}");
        }

        return lyricsResponseBuilder.ToString();
    }
}