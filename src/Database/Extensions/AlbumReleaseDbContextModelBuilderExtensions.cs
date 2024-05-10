using Microsoft.EntityFrameworkCore;

using MuzakBot.Lib.Models.Database.AlbumRelease;

namespace MuzakBot.Database.Extensions;

/// <summary>
/// Extension methods for creating the models for <see cref="AlbumReleaseDbContext"/>.
/// </summary>
public static class AlbumReleaseDbContextModelBuilderExtensions
{
    /// <summary>
    /// Creates the model for the <see cref="AlbumReleaseReminder"/> entity.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The model builder.</returns>
    public static ModelBuilder CreateAlbumReleaseReminderModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlbumReleaseReminder>(entity =>
        {
            entity.ToTable("release_reminders");
            entity.ToContainer("release_reminders");

            entity.HasKey(e => e.Id);
            entity.HasPartitionKey(e => e.PartitionKey);

            entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ToJsonProperty("id");

            entity
                .Property(e => e.PartitionKey)
                .HasColumnName("partitionKey")
                .ToJsonProperty("partitionKey");

            entity
                .Property(e => e.AlbumId)
                .HasColumnName("albumId")
                .ToJsonProperty("albumId");

            entity
                .Property(e => e.GuildId)
                .HasColumnName("guildId")
                .ToJsonProperty("guildId");

            entity
                .Property(e => e.ChannelId)
                .HasColumnName("channelId")
                .ToJsonProperty("channelId");

            entity
                .Property(e => e.UserIdsToRemind)
                .HasColumnName("userIdsToRemind")
                .ToJsonProperty("userIdsToRemind");

            entity
                .Property(e => e.ReleaseDate)
                .HasColumnName("releaseDate")
                .ToJsonProperty("releaseDate");

            entity
                .Property(e => e.ReminderSent)
                .HasColumnName("reminderSent")
                .ToJsonProperty("reminderSent");
        });

        return modelBuilder;
    }
}