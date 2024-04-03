using Microsoft.EntityFrameworkCore;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Database;

public class SongLyricsDbContext : DbContext
{
    public SongLyricsDbContext(DbContextOptions<SongLyricsDbContext> options) : base(options)
    {}

    public DbSet<SongLyricsItem> SongLyricsItems { get; set; } = null!;
}
