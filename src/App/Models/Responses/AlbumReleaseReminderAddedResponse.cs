using Discord;

using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response for when an album release reminder is added for a user. 
/// </summary>
public sealed class AlbumReleaseReminderAddedResponse : IResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseReminderAddedResponse"/> class.
    /// </summary>
    /// <param name="album">The album that the reminder was added for.</param>
    public AlbumReleaseReminderAddedResponse(Album album)
    {
        Album = album;
    }

    /// <summary>
    /// The album that the reminder was added for.
    /// </summary>
    public Album Album { get; }

    /// <inheritdoc/>
    public ComponentBuilder GenerateComponent()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public EmbedBuilder GenerateEmbed()
    {
        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle("🔔 Reminder added")
            .WithDescription($"You will be reminded when **{Album.Attributes!.Name}** is released!")
            .WithColor(Color.Green);

        return embedBuilder;
    }

    /// <inheritdoc/>
    public string GenerateText()
    {
        throw new NotImplementedException();
    }
}
