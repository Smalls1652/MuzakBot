using Discord;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Generates a <see cref="EmbedBuilder"/> for the music share message.
    /// </summary>
    /// <param name="streamingEntityItem">The streaming service data for the song/album.</param>
    /// <returns></returns>
    private EmbedBuilder GenerateEmbedBuilder(StreamingEntityItem streamingEntityItem)
    {
        return new EmbedBuilder()
            .WithTitle(streamingEntityItem.Title)
            .WithDescription($"by {streamingEntityItem.ArtistName}")
            .WithColor(Color.DarkBlue)
            .WithImageUrl($"attachment://{streamingEntityItem.Id}.jpg")
            .WithFooter("(Powered by Songlink/Odesli)");
    }
}