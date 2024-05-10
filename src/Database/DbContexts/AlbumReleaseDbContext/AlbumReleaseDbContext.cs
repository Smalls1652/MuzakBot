using Microsoft.EntityFrameworkCore;

using MuzakBot.Database.Extensions;
using MuzakBot.Lib.Models.Database.AlbumRelease;

namespace MuzakBot.Database;

/// <summary>
/// Database context for album release reminders.
/// </summary>
public class AlbumReleaseDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlbumReleaseDbContext"/> class.
    /// </summary>
    /// <param name="options"></param>
    public AlbumReleaseDbContext(DbContextOptions<AlbumReleaseDbContext> options) : base(options)
    {}

    /// <summary>
    /// <see cref="AlbumReleaseReminder"/> items in the database.
    /// </summary>
    public DbSet<AlbumReleaseReminder> AlbumReleaseReminders { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .CreateAlbumReleaseReminderModel();
    }
}
