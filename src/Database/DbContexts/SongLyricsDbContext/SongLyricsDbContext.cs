using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using MuzakBot.Database.Extensions;

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

    /// <summary>
    /// <see cref="LyricsAnalyzerItem"/> items in the database.
    /// </summary>
    public DbSet<LyricsAnalyzerItem> LyricsAnalyzerItems { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .CreateSongLyricsItemModel()
            .CreateSongLyricsRequestJobModel()
            .CreateLyricsAnalyzerItemModel();
    }    
}
