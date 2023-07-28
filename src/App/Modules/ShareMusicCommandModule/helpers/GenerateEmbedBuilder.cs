using Discord;
using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
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