using Microsoft.EntityFrameworkCore;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Database;

/// <summary>
/// Database context for song lyrics.
/// </summary>
public class SongLyricsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongLyricsDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for the database context.</param>
    public SongLyricsDbContext(DbContextOptions<SongLyricsDbContext> options) : base(options)
    {}

    /// <summary>
    /// <see cref="SongLyricsItem"/> items in the database.
    /// </summary>
    public DbSet<SongLyricsItem> SongLyricsItems { get; set; } = null!;

    /// <summary>
    /// <see cref="SongLyricsRequestJob"/> items in the database.
    /// </summary>
    public DbSet<SongLyricsRequestJob> SongLyricsRequestJobs { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .CreateSongLyricsItemModel()
            .CreateSongLyricsRequestJobModel();
    }    
}

/// <summary>
/// Internal extension methods for creating models for the <see cref="SongLyricsDbContext"/> class.
/// </summary>
internal static class SongLyricsDbContext_ModelBuilderExtensions
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
}
