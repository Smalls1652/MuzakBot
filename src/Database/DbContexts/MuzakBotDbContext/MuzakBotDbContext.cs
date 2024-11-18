using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MuzakBot.Database.Models;
using MuzakBot.Lib.Models.Database;
using MuzakBot.Lib.Models.Database.AlbumRelease;
using MuzakBot.Lib.Models.Database.LyricsAnalyzer;

namespace MuzakBot.Database;

/// <summary>
/// Database context for MuzakBot.
/// </summary>
public class MuzakBotDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MuzakBotDbContext"/> class.
    /// </summary>
    /// <param name="options"></param>
    public MuzakBotDbContext(DbContextOptions<MuzakBotDbContext> options) : base(options)
    { }

    /// <summary>
    /// <see cref="DatabaseUpdate"/> items in the database.
    /// </summary>
    public DbSet<DatabaseUpdate> DatabaseUpdates { get; set; } = null!;

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

    /// <summary>
    /// <see cref="AnalyzedLyrics"/> items in the database.
    /// </summary>
    public DbSet<AnalyzedLyrics> AnalyzedLyricsItems { get; set; } = null!;

    /// <summary>
    /// <see cref="AlbumReleaseReminder"/> items in the database.
    /// </summary>
    public DbSet<AlbumReleaseReminder> AlbumReleaseReminders { get; set; } = null!;
}

public static class MuzakBotDbContextExtension
{
    /// <summary>
    /// Adds a <see cref="MuzakBotDbContext"/> factory to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="databaseConfig">The database configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an invalid database type is found.</exception>
    public static IServiceCollection AddMuzakBotDbContextFactory(this IServiceCollection services, DatabaseConfig databaseConfig)
    {
        switch (databaseConfig.DatabaseType)
        {
            case DatabaseType.PostgreSql:
                services.AddMuzakBotDbContextFactoryForPostgres(databaseConfig.PostgresDbConfig!);
                break;

            case DatabaseType.Sqlite:
                services.AddMuzakBotDbContextFactoryForSqlite(databaseConfig.SqliteDbConfig!);
                break;

            default:
                throw new InvalidOperationException("Invalid database type.");
        }

        return services;
    }

    /// <summary>
    /// Adds a <see cref="MuzakBotDbContext"/> factory and configures it for PostgreSQL to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="postgresConfig">The PostgreSQL configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddMuzakBotDbContextFactoryForPostgres(this IServiceCollection services, PostgresConfigOptions postgresConfig)
    {
        services.AddDbContextFactory<MuzakBotDbContext>(options =>
        {
            options.UseNpgsql(postgresConfig.ConnectionString);
        });

        return services;
    }

    /// <summary>
    /// Adds a <see cref="MuzakBotDbContext"/> factory and configures it for SQLite to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="sqliteDbConfig">The SQLite DB configuration.</param>
    /// <returns>The same service collection so that multiple calls can be chained.</returns>
    private static IServiceCollection AddMuzakBotDbContextFactoryForSqlite(this IServiceCollection services, SqliteDbConfigOptions sqliteDbConfig)
    {
        string dbPath = Path.Join(sqliteDbConfig.DbPath, "muzakbot.sqlite");

        services.AddDbContextFactory<MuzakBotDbContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        return services;
    }

    /// <summary>
    /// Applies migrations for <see cref="MuzakBotDbContext"/> to the database.
    /// </summary>
    /// <remarks>
    /// This method will not apply migrations if the database is not relational.
    /// </remarks>
    /// <param name="host">The host.</param>
    /// <returns></returns>
    public static async Task ApplyMuzakBotDbContextMigrations(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MuzakBotDbContext>>();

        using var context = dbContextFactory.CreateDbContext();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
            await context.SaveChangesAsync();
        }
    }
}
