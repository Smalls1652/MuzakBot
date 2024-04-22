using Microsoft.EntityFrameworkCore;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Database.Extensions;

internal static class SongLyricsDbContextModelBuilderExtensions
{
    /// <summary>
    /// Creates the model for the <see cref="SongLyricsItem"/> class.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static ModelBuilder CreateSongLyricsItemModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SongLyricsItem>(entity =>
        {
            entity.ToTable("song_lyrics");
            entity.ToContainer("song_lyrics");

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
                .Property(e => e.ArtistName)
                .HasColumnName("artistName")
                .ToJsonProperty("artistName");

            entity
                .Property(e => e.SongName)
                .HasColumnName("songName")
                .ToJsonProperty("songName");

            entity
                .Property(e => e.Lyrics)
                .HasColumnName("lyrics")
                .ToJsonProperty("lyrics");

            entity
                .Property(e => e.CreatedAt)
                .HasColumnName("createdAt")
                .ToJsonProperty("createdAt");
        });

        return modelBuilder;
    }

    /// <summary>
    /// Creates the model for the <see cref="SongLyricsRequestJob"/> class.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static ModelBuilder CreateSongLyricsRequestJobModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SongLyricsRequestJob>(entity =>
        {
            entity.ToTable("song_lyrics_request_jobs");
            entity.ToContainer("song_lyrics_request_jobs");

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
                .Property(e => e.GeniusUrl)
                .HasColumnName("geniusUrl")
                .ToJsonProperty("geniusUrl");

            entity
                .Property(e => e.CreatedAt)
                .HasColumnName("createdAt")
                .ToJsonProperty("createdAt");

            entity
                .Property(e => e.StandaloneServiceAcknowledged)
                .HasColumnName("standaloneServiceAcknowledged")
                .ToJsonProperty("standaloneServiceAcknowledged");

            entity
                .Property(e => e.FallbackMethodNeeded)
                .HasColumnName("fallbackMethodNeeded")
                .ToJsonProperty("fallbackMethodNeeded");

            entity
                .Property(e => e.IsCompleted)
                .HasColumnName("isCompleted")
                .ToJsonProperty("isCompleted");

            entity
                .Property(e => e.SongLyricsItemId)
                .HasColumnName("songLyricsItemId")
                .ToJsonProperty("songLyricsItemId");
        });

        return modelBuilder;
    }

    public static ModelBuilder CreateLyricsAnalyzerItemModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LyricsAnalyzerItem>(entity =>
        {
            entity.ToTable("lyrics_analyzer_items");
            entity.ToContainer("lyrics_analyzer_items");

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
                .Property(e => e.ArtistName)
                .HasColumnName("artistName")
                .ToJsonProperty("artistName");

            entity
                .Property(e => e.SongName)
                .HasColumnName("songName")
                .ToJsonProperty("songName");

            entity
                .Property(e => e.PromptStyle)
                .HasColumnName("promptStyle")
                .ToJsonProperty("promptStyle");

            entity
                .Property(e => e.CreatedAt)
                .HasColumnName("createdAt")
                .ToJsonProperty("createdAt");
        });

        return modelBuilder;
    }
}
