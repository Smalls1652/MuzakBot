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

    /// <summary>
    /// Creates the model for the <see cref="LyricsAnalyzerItem"/> class.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
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

    /// <summary>
    /// Creates the model for the <see cref="LyricsAnalyzerConfig"/> class.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static ModelBuilder CreateLyricsAnalyzerConfigModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LyricsAnalyzerConfig>(entity =>
        {
            entity.ToTable("command_configs");
            entity.ToContainer("command_configs");

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
                .Property(e => e.RateLimitEnabled)
                .HasColumnName("rateLimitEnabled")
                .ToJsonProperty("rateLimitEnabled");

            entity
                .Property(e => e.RateLimitMaxRequests)
                .HasColumnName("rateLimitMaxRequests")
                .ToJsonProperty("rateLimitMaxRequests");

            entity
                .Property(e => e.RateLimitIgnoredUserIds)
                .HasColumnName("rateLimitIgnoredUserIds")
                .ToJsonProperty("rateLimitIgnoredUserIds");

            entity
                .Property(e => e.CommandIsEnabledToSpecificGuilds)
                .HasColumnName("commandIsEnabledToSpecificGuilds")
                .ToJsonProperty("commandIsEnabledToSpecificGuilds");

            entity
                .Property(e => e.CommandEnabledGuildIds)
                .HasColumnName("commandEnabledGuildIds")
                .ToJsonProperty("commandEnabledGuildIds");

            entity
                .Property(e => e.CommandDisabledGuildIds)
                .HasColumnName("commandDisabledGuildIds")
                .ToJsonProperty("commandDisabledGuildIds");
        });

        return modelBuilder;
    }

    /// <summary>
    /// Creates the model for the <see cref="LyricsAnalyzerPromptStyle"/> class.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static ModelBuilder CreateLyricsAnalyzerPromptStyleModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LyricsAnalyzerPromptStyle>(entity =>
        {
            entity.ToTable("prompt_styles");
            entity.ToContainer("prompt_styles");

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
                .Property(e => e.Name)
                .HasColumnName("name")
                .ToJsonProperty("name");

            entity
                .Property(e => e.ShortName)
                .HasColumnName("shortName")
                .ToJsonProperty("shortName");

            entity
                .Property(e => e.AnalysisType)
                .HasColumnName("analysisType")
                .ToJsonProperty("analysisType");

            entity
                .Property(e => e.Prompt)
                .HasColumnName("prompt")
                .ToJsonProperty("prompt");

            entity
                .Property(e => e.NoticeText)
                .HasColumnName("noticeText")
                .ToJsonProperty("noticeText");

            entity
                .Property(e => e.CreatedOn)
                .HasColumnName("createdOn")
                .ToJsonProperty("createdOn");

            entity
                .Property(e => e.LastUpdated)
                .HasColumnName("lastUpdated")
                .ToJsonProperty("lastUpdated");
        });

        return modelBuilder;
    }

    /// <summary>
    /// Creates the model for the <see cref="LyricsAnalyzerUserRateLimit"/> class.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    /// <returns>The same builder instance so that multiple calls can be chained.</returns>
    public static ModelBuilder CreateLyricsAnalyzerUserRateLimitModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LyricsAnalyzerUserRateLimit>(entity =>
        {
            entity.ToTable("user_rate_limit");
            entity.ToContainer("user_rate_limit");

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
                .Property(e => e.UserId)
                .HasColumnName("userId")
                .ToJsonProperty("userId");

            entity
                .Property(e => e.CurrentRequestCount)
                .HasColumnName("currentRequestCount")
                .ToJsonProperty("currentRequestCount");

            entity
                .Property(e => e.LastRequestTime)
                .HasColumnName("lastRequestTime")
                .ToJsonProperty("lastRequestTime");
        });

        return modelBuilder;
    }
}
