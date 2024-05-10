using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database.AlbumRelease;

[Table("release_reminders")]
public class AlbumReleaseReminder : DatabaseItem
{
    [JsonConstructor]
    public AlbumReleaseReminder()
    {}

    public AlbumReleaseReminder(string albumId, string guildId, string channelId, DateTimeOffset releaseDate)
    {
        Id = $"{albumId}-{guildId}-{channelId}";
        PartitionKey = "album-release-reminder";
        AlbumId = albumId;
        GuildId = guildId;
        ChannelId = channelId;
        ReleaseDate = releaseDate.UtcDateTime;
    }

    [Column("albumId")]
    [JsonPropertyName("albumId")]
    public string AlbumId { get; set; } = null!;

    [Column("guildId")]
    [JsonPropertyName("guildId")]
    public string GuildId { get; set; }

    [Column("channelId")]
    [JsonPropertyName("channelId")]
    public string ChannelId { get; set; }

    [Column("userIdsToRemind")]
    [JsonPropertyName("userIdsToRemind")]
    public List<string> UserIdsToRemind { get; set; } = [];

    [Column("releaseDate")]
    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    [Column("reminderSent")]
    [JsonPropertyName("reminderSent")]
    public bool ReminderSent { get; set; } = false;

    public void AddUserIdToRemind(string userId)
    {
        if (UserIdsToRemind.Contains(userId))
        {
            throw new InvalidOperationException("User ID is already in the list of user IDs to remind.");
        }

        UserIdsToRemind.Add(userId);
    }

    public void RemoveUserIdToRemind(string userId)
    {
        if (!UserIdsToRemind.Contains(userId))
        {
            throw new InvalidOperationException("User ID is not in the list of user IDs to remind.");
        }

        UserIdsToRemind.Remove(userId);
    }
}
