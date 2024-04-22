using Microsoft.EntityFrameworkCore;

using MuzakBot.Database.Extensions;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Database;

/// <summary>
/// Database context for song lyrics.
/// </summary>
public class LyricsAnalyzerDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LyricsAnalyzerDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for the database context.</param>
    public LyricsAnalyzerDbContext(DbContextOptions<LyricsAnalyzerDbContext> options) : base(options)
    {}

    /// <summary>
    /// <see cref="SongLyricsItem"/> items in the database.
    /// </summary>
    public DbSet<SongLyricsItem> SongLyricsItems { get; set; } = null!;

    /// <summary>
    /// <see cref="SongLyricsRequestJob"/> items in the database.
    /// </summary>
    public DbSet<SongLyricsRequestJob> SongLyricsRequestJobs { get; set; } = null!;

    /// <summary>
    /// <see cref="LyricsAnalyzerItem"/> items in the database.
    /// </summary>
    public DbSet<LyricsAnalyzerItem> LyricsAnalyzerItems { get; set; } = null!;

    /// <summary>
    /// <see cref="LyricsAnalyzerConfig"/> items in the database.
    /// </summary>
    public DbSet<LyricsAnalyzerConfig> LyricsAnalyzerConfigs { get; set; } = null!;

    /// <summary>
    /// <see cref="LyricsAnalyzerPromptStyle"/> items in the database.
    /// </summary>
    public DbSet<LyricsAnalyzerPromptStyle> LyricsAnalyzerPromptStyles { get; set; } = null!;

    /// <summary>
    /// <see cref="LyricsAnalyzerUserRateLimit"/> items in the database.
    /// </summary>
    public DbSet<LyricsAnalyzerUserRateLimit> LyricsAnalyzerUserRateLimits { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .CreateSongLyricsItemModel()
            .CreateSongLyricsRequestJobModel()
            .CreateLyricsAnalyzerItemModel()
            .CreateLyricsAnalyzerConfigModel()
            .CreateLyricsAnalyzerPromptStyleModel()
            .CreateLyricsAnalyzerUserRateLimitModel();
    }    
}
