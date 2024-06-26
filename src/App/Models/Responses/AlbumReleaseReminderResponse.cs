using System.Text;

using Discord;
using Discord.WebSocket;

using MuzakBot.Lib.Models.AppleMusic;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response for the album release reminder.
/// </summary>
public sealed class AlbumReleaseReminderResponse : IBotResponse, IDisposable
{
    private bool _isDisposed;

    /// <summary>
    /// Initialize a new instance of the <see cref="AlbumReleaseReminderResponse"/> class.
    /// </summary>
    /// <param name="album">The album to show the release date for.</param>
    /// <param name="musicEntityItem">The music entity item from Odesli.</param>
    /// <param name="usersToNotify">The users to notify.</param>
    public AlbumReleaseReminderResponse(Album album, MusicEntityItem musicEntityItem, List<string> usersToNotify)
    {
        Album = album;
        MusicEntityItem = musicEntityItem;
        UsersToNotify = usersToNotify;
        AlbumArtFileName = $"{Guid.NewGuid():N}.jpg";
        AlbumArtworkStream = Album.Attributes!.Artwork.GetAlbumArtworkStreamAsync(512, 512).GetAwaiter().GetResult();
    }

    /// <summary>
    /// The album to show the release date for.
    /// </summary>
    public Album Album { get; }

    /// <summary>
    /// The music entity item from Odesli.
    /// </summary>
    public MusicEntityItem MusicEntityItem { get; }

    /// <summary>
    /// The users to notify.
    /// </summary>
    public List<string> UsersToNotify { get; }

    /// <summary>
    /// The name being used for the album artwork file.
    /// </summary>
    public string AlbumArtFileName { get; }

    /// <summary>
    /// The stream of the album artwork.
    /// </summary>
    public Stream AlbumArtworkStream { get; }

    /// <inheritdoc/>
    public ComponentBuilder GenerateComponent()
    {
        ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithButton(
                label: "Links",
                style: ButtonStyle.Link,
                url: MusicEntityItem.PageUrl!.ToString()
            );

        return componentBuilder;
    }

    /// <inheritdoc/>
    public EmbedBuilder GenerateEmbed()
    {
        StringBuilder descriptionBuilder = new StringBuilder();

        descriptionBuilder
            .AppendLine($"by {Album.Attributes!.ArtistName}")
            .AppendLine()
            .AppendLine("🔔 **Album has been released!** 🔔")
            .AppendLine()
            .AppendLine(string.Join(", ", UsersToNotify));

        EmbedBuilder embedBuilder = new EmbedBuilder()
            .WithTitle(Album.Attributes!.Name)
            .WithDescription(descriptionBuilder.ToString())
            .WithColor(Color.Green)
            .WithImageUrl($"attachment://{AlbumArtFileName}");

        return embedBuilder;
    }

    /// <inheritdoc/>
    public string GenerateText()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        AlbumArtworkStream.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
