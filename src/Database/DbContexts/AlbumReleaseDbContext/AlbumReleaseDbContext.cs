using Microsoft.EntityFrameworkCore;

using MuzakBot.Database.Extensions;
using MuzakBot.Lib.Models.Database.AlbumRelease;

namespace MuzakBot.Database;

public class AlbumReleaseDbContext : DbContext
{
    public AlbumReleaseDbContext(DbContextOptions<AlbumReleaseDbContext> options) : base(options)
    {}

    public DbSet<AlbumReleaseReminder> AlbumReleaseReminders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .CreateAlbumReleaseReminderModel();
    }
}
