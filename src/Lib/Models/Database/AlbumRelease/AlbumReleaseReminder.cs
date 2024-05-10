using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database.AlbumRelease;

[Table("release_reminders")]
public class AlbumReleaseReminder : DatabaseItem
{
    [JsonConstructor]
    public AlbumReleaseReminder()
    {}

    public AlbumReleaseReminder(string albumId, ulong guildId, DateTimeOffset releaseDate)
    {
        Id = new Guid($"{albumId}-{guildId}").ToString();
        PartitionKey = "album-release-reminder";
        AlbumId = albumId;
        GuildId = guildId;
        ReleaseDate = releaseDate.UtcDateTime;
    }

    [Column("albumId")]
    [JsonPropertyName("albumId")]
    public string AlbumId { get; set; } = null!;

    [Column("guildId")]
    [JsonPropertyName("guildId")]
    public ulong GuildId { get; set; }

    [Column("userIdsToRemind")]
    [JsonPropertyName("userIdsToRemind")]
    public List<string> UserIdsToRemind { get; set; } = [];

    [Column("releaseDate")]
    [JsonPropertyName("releaseDate")]
    public DateTime ReleaseDate { get; set; }

    [Column("reminderSent")]
    [JsonPropertyName("reminderSent")]
    public bool ReminderSent { get; set; } = false;
}
