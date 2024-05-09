using Microsoft.EntityFrameworkCore;

using MuzakBot.Lib.Models.Database.AlbumRelease;

namespace MuzakBot.Database.Extensions;

public static class AlbumReleaseDbContextModelBuilderExtensions
{
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
                .Property(e => e.ArtistId)
                .HasColumnName("artistId")
                .ToJsonProperty("artistId");

            entity
                .Property(e => e.AlbumId)
                .HasColumnName("albumId")
                .ToJsonProperty("albumId");

            entity
                .Property(e => e.GuildId)
                .HasColumnName("guildId")
                .ToJsonProperty("guildId");

            entity
                .Property(e => e.UserIdsToRemind)
                .HasColumnName("userIdsToRemind")
                .ToJsonProperty("userIdsToRemind");

            entity
                .Property(e => e.ReleaseDate)
                .HasColumnName("releaseDate")
                .ToJsonProperty("releaseDate");
        });

        return modelBuilder;
    }
}
