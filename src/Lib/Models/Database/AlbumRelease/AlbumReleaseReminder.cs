using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database.AlbumRelease;

/// <summary>
/// Represents an album release reminder.
/// </summary>
[Table("release_reminders")]
public class AlbumReleaseReminder : DatabaseItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseReminder"/> class.
    /// </summary>
    [JsonConstructor]
    public AlbumReleaseReminder()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseReminder"/> class.
    /// </summary>
    /// <param name="albumId">The ID of the album on Apple Music.</param>
    /// <param name="guildId">The ID of the guild.</param>
    /// <param name="channelId">The ID of the channel in the guild.</param>
    /// <param name="releaseDate">The release date of the album.</param>
    public AlbumReleaseReminder(string albumId, string guildId, string channelId, DateTimeOffset releaseDate)
    {
        Id = $"{albumId}-{guildId}-{channelId}";
        PartitionKey = "album-release-reminder";
        AlbumId = albumId;
        GuildId = guildId;
        ChannelId = channelId;
        ReleaseDate = releaseDate.UtcDateTime;
    }

    /// <summary>
    /// The ID of the album on Apple Music.
    /// </summary>
    [Column("albumId")]
    [JsonPropertyName("albumId")]
    public string AlbumId { get; set; } = null!;

    /// <summary>
    /// The ID of the guild.
    /// </summary>
    [Column("guildId")]
    [JsonPropertyName("guildId")]
    public string GuildId { get; set; } = null!;

    /// <summary>
    /// The ID of the channel in the guild.
    /// </summary>
    [Column("channelId")]
    [JsonPropertyName("channelId")]
    public string ChannelId { get; set; } = null!;

    /// <summary>
    /// The user IDs to remind.
    /// </summary>
    [Column("userIdsToRemind")]
    [JsonPropertyName("userIdsToRemind")]
    public List<string> UserIdsToRemind { get; set; } = [];

    /// <summary>
    /// The release date of the album.
    /// </summary>
    [Column("releaseDate")]
    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Whether the reminder has been sent.
    /// </summary>
    [Column("reminderSent")]
    [JsonPropertyName("reminderSent")]
    public bool ReminderSent { get; set; } = false;

    /// <summary>
    /// Adds a user ID to remind.
    /// </summary>
    /// <param name="userId">The user ID of the user.</param>
    /// <exception cref="InvalidOperationException">Thrown when the user ID is already in the list of user IDs to remind.</exception>
    public void AddUserIdToRemind(string userId)
    {
        if (UserIdsToRemind.Contains(userId))
        {
            throw new InvalidOperationException("User ID is already in the list of user IDs to remind.");
        }

        UserIdsToRemind.Add(userId);
    }

    /// <summary>
    /// Removes a user ID to remind.
    /// </summary>
    /// <param name="userId">The user ID of the user.</param>
    /// <exception cref="InvalidOperationException">Thrown when the user ID is not in the list of user IDs to remind.</exception>
    public void RemoveUserIdToRemind(string userId)
    {
        if (!UserIdsToRemind.Contains(userId))
        {
            throw new InvalidOperationException("User ID is not in the list of user IDs to remind.");
        }

        UserIdsToRemind.Remove(userId);
    }
}
