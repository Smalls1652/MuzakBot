using Discord;

using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.App.Models.Responses;

public sealed class AlbumReleaseReminderAddedResponse : IResponse
{
    public AlbumReleaseReminderAddedResponse(Album album)
    {
        Album = album;
    }

    public Album Album { get; }

    public ComponentBuilder GenerateComponent()
    {
        throw new NotImplementedException();
    }

    public EmbedBuilder GenerateEmbed()
    {
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle("🔔 Reminder added")
            .WithDescription($"You will be reminded when **{Album.Attributes!.Name}** is released!")
            .WithColor(Color.Green);

        return embedBuilder;
    }

    public string GenerateText()
    {
        throw new NotImplementedException();
    }
}
