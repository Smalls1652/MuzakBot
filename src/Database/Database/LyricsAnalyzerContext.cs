using Microsoft.EntityFrameworkCore;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Database;

public class LyricsAnalyzerContext : DbContext
{
    public LyricsAnalyzerContext(DbContextOptions<LyricsAnalyzerContext> options) : base(options)
    {}

    public DbSet<SongLyricsItem> SongLyrics { get; set; } = null!;
    public DbSet<LyricsAnalyzerConfig> Configs { get; set; } = null!;
    public DbSet<LyricsAnalyzerPromptStyle> PromptStyles { get; set; } = null!;
    public DbSet<LyricsAnalyzerUserRateLimit> UserRateLimits { get; set; } = null!;
}
