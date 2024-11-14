using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

        services.AddDbContextFactory<AlbumReleaseDbContext>(options =>
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

    /// <summary>
    /// Migrates data from CosmosDB to Postgres.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <returns></returns>
    public static async Task MigrateCosmosDbDataToPostgresAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var albumreleaseDbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AlbumReleaseDbContext>>();
        var lyricsanalyzerDbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<LyricsAnalyzerDbContext>>();
        var muzakBotDbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MuzakBotDbContext>>();

        using var albumReleaseContext = albumreleaseDbContextFactory.CreateDbContext();
        using var lyricsAnalyzerContext = lyricsanalyzerDbContextFactory.CreateDbContext();
        using var muzakBotContext = muzakBotDbContextFactory.CreateDbContext();

        using ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("MuzakBot.Database.MigrateCosmosDbDataToPostgres");

        logger.LogInformation("Migrating data from CosmosDB to Postgres...");

        // Migrate album release reminders
        await MigrateAlbumReleaseRemindersAsync(logger, muzakBotContext, albumReleaseContext);

        // Migrate lyrics analyzer prompt styles
        await MigrateLyricsAnalyzerPromptStylesAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate analyzed lyrics
        await MigrateAnalyzedLyricsAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate lyrics analyzer command configs
        await MigrateLyricsAnalyzerCommandConfigsAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate lyrics analyzer user rate limits
        await MigrateLyricsAnalyzerUserRateLimitsAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate song lyrics items
        await MigrateSongLyricsItemsAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate song lyrics request jobs
        await MigrateSongLyricsRequestJobsAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate database updates
        await MigrateDatabaseUpdatesAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        // Migrate lyrics analyzer items
        await MigrateLyricsAnalyzerItemsAsync(logger, muzakBotContext, lyricsAnalyzerContext);

        logger.LogInformation("Data migration from CosmosDB to Postgres complete.");
    }

    /// <summary>
    /// Migrates AlbumReleaseReminders from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="albumReleaseDbContext">The <see cref="AlbumReleaseDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateAlbumReleaseRemindersAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, AlbumReleaseDbContext albumReleaseDbContext)
    {
        int albumReleaseRemindersPostgresCount = await muzakBotDbContext.AlbumReleaseReminders.CountAsync();

        if (albumReleaseRemindersPostgresCount > 0)
        {
            logger.LogWarning("AlbumReleaseReminders table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating AlbumReleaseReminders from CosmosDB to Postgres...");
        AlbumReleaseReminder[] albumReleaseRemindersCosmosAll = await albumReleaseDbContext.AlbumReleaseReminders.ToArrayAsync();

        foreach (AlbumReleaseReminder albumReleaseReminder in albumReleaseRemindersCosmosAll)
        {
            muzakBotDbContext.AlbumReleaseReminders.Add(albumReleaseReminder);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated AlbumReleaseReminders from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates AnalyzedLyricsItems from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateAnalyzedLyricsAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int analyzedLyricsPostgresCount = await muzakBotDbContext.AnalyzedLyricsItems.CountAsync();

        if (analyzedLyricsPostgresCount > 0)
        {
            logger.LogWarning("AnalyzedLyricsItems table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating AnalyzedLyricsItems from CosmosDB to Postgres...");
        AnalyzedLyrics[] analyzedLyricsCosmosAll = await lyricsAnalyzerDbContext.AnalyzedLyricsItems.ToArrayAsync();

        foreach (AnalyzedLyrics analyzedLyrics in analyzedLyricsCosmosAll)
        {
            muzakBotDbContext.AnalyzedLyricsItems.Add(analyzedLyrics);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated AnalyzedLyricsItems from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates LyricsAnalyzerConfigs from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateLyricsAnalyzerCommandConfigsAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int lyricsAnalyzerConfigsPostgresCount = await muzakBotDbContext.LyricsAnalyzerConfigs.CountAsync();

        if (lyricsAnalyzerConfigsPostgresCount > 0)
        {
            logger.LogWarning("LyricsAnalyzerConfigs table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating LyricsAnalyzerConfigs from CosmosDB to Postgres...");
        LyricsAnalyzerConfig[] lyricsAnalyzerConfigsCosmosAll = await lyricsAnalyzerDbContext.LyricsAnalyzerConfigs.ToArrayAsync();

        foreach (LyricsAnalyzerConfig lyricsAnalyzerConfig in lyricsAnalyzerConfigsCosmosAll)
        {
            muzakBotDbContext.LyricsAnalyzerConfigs.Add(lyricsAnalyzerConfig);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated LyricsAnalyzerConfigs from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates LyricsAnalyzerPromptStyles from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateLyricsAnalyzerPromptStylesAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int lyricsAnalyzerPromptStylesPostgresCount = await muzakBotDbContext.LyricsAnalyzerPromptStyles.CountAsync();

        if (lyricsAnalyzerPromptStylesPostgresCount > 0)
        {
            logger.LogWarning("LyricsAnalyzerPromptStyles table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating LyricsAnalyzerPromptStyles from CosmosDB to Postgres...");

        List<LyricsAnalyzerPromptStyle> lyricsAnalyzerPromptStylesCosmosAll = await lyricsAnalyzerDbContext.LyricsAnalyzerPromptStyles.ToListAsync();

        foreach (LyricsAnalyzerPromptStyle lyricsAnalyzerPromptStyle in lyricsAnalyzerPromptStylesCosmosAll)
        {
            lyricsAnalyzerPromptStyle.CreatedOn = lyricsAnalyzerPromptStyle.CreatedOn.ToUniversalTime();
            lyricsAnalyzerPromptStyle.LastUpdated = lyricsAnalyzerPromptStyle.LastUpdated.ToUniversalTime();
            muzakBotDbContext.LyricsAnalyzerPromptStyles.Add(lyricsAnalyzerPromptStyle);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated LyricsAnalyzerPromptStyles from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates LyricsAnalyzerUserRateLimits from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateLyricsAnalyzerUserRateLimitsAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int lyricsAnalyzerUserRateLimitsPostgresCount = await muzakBotDbContext.LyricsAnalyzerUserRateLimits.CountAsync();

        if (lyricsAnalyzerUserRateLimitsPostgresCount > 0)
        {
            logger.LogWarning("LyricsAnalyzerUserRateLimits table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating LyricsAnalyzerUserRateLimits from CosmosDB to Postgres...");
        LyricsAnalyzerUserRateLimit[] lyricsAnalyzerUserRateLimitsCosmosAll = await lyricsAnalyzerDbContext.LyricsAnalyzerUserRateLimits.ToArrayAsync();

        foreach (LyricsAnalyzerUserRateLimit lyricsAnalyzerUserRateLimit in lyricsAnalyzerUserRateLimitsCosmosAll)
        {
            muzakBotDbContext.LyricsAnalyzerUserRateLimits.Add(lyricsAnalyzerUserRateLimit);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated LyricsAnalyzerUserRateLimits from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates SongLyricsItems from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateSongLyricsItemsAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int songLyricsItemsPostgresCount = await muzakBotDbContext.SongLyricsItems.CountAsync();

        if (songLyricsItemsPostgresCount > 0)
        {
            logger.LogWarning("SongLyricsItems table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating SongLyricsItems from CosmosDB to Postgres...");
        SongLyricsItem[] songLyricsItemsCosmosAll = await lyricsAnalyzerDbContext.SongLyricsItems.ToArrayAsync();

        foreach (SongLyricsItem songLyricsItem in songLyricsItemsCosmosAll)
        {
            muzakBotDbContext.SongLyricsItems.Add(songLyricsItem);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated SongLyricsItems from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates SongLyricsRequestJobs from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateSongLyricsRequestJobsAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int songLyricsRequestJobsPostgresCount = await muzakBotDbContext.SongLyricsRequestJobs.CountAsync();

        if (songLyricsRequestJobsPostgresCount > 0)
        {
            logger.LogWarning("SongLyricsRequestJobs table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating SongLyricsRequestJobs from CosmosDB to Postgres...");
        SongLyricsRequestJob[] songLyricsRequestJobsCosmosAll = await lyricsAnalyzerDbContext.SongLyricsRequestJobs.ToArrayAsync();

        foreach (SongLyricsRequestJob songLyricsRequestJob in songLyricsRequestJobsCosmosAll)
        {
            muzakBotDbContext.SongLyricsRequestJobs.Add(songLyricsRequestJob);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated SongLyricsRequestJobs from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates DatabaseUpdates from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateDatabaseUpdatesAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int databaseUpdatesPostgresCount = await muzakBotDbContext.DatabaseUpdates.CountAsync();

        if (databaseUpdatesPostgresCount > 0)
        {
            logger.LogWarning("DatabaseUpdates table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating DatabaseUpdates from CosmosDB to Postgres...");
        DatabaseUpdate[] databaseUpdatesCosmosAll = await lyricsAnalyzerDbContext.DatabaseUpdates.ToArrayAsync();

        foreach (DatabaseUpdate databaseUpdate in databaseUpdatesCosmosAll)
        {
            muzakBotDbContext.DatabaseUpdates.Add(databaseUpdate);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated DatabaseUpdates from CosmosDB to Postgres.");
    }

    /// <summary>
    /// Migrates LyricsAnalyzerItems from CosmosDB to Postgres.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="muzakBotDbContext">The <see cref="MuzakBotDbContext"/> instance.</param>
    /// <param name="lyricsAnalyzerDbContext">The <see cref="LyricsAnalyzerDbContext"/> instance.</param>
    /// <returns></returns>
    private static async Task MigrateLyricsAnalyzerItemsAsync(ILogger logger, MuzakBotDbContext muzakBotDbContext, LyricsAnalyzerDbContext lyricsAnalyzerDbContext)
    {
        int lyricsAnalyzerItemsPostgresCount = await muzakBotDbContext.LyricsAnalyzerItems.CountAsync();

        if (lyricsAnalyzerItemsPostgresCount > 0)
        {
            logger.LogWarning("LyricsAnalyzerItems table in Postgres is not empty. Skipping migration.");
            return;
        }

        logger.LogInformation("Migrating LyricsAnalyzerItems from CosmosDB to Postgres...");
        LyricsAnalyzerItem[] lyricsAnalyzerItemsCosmosAll = await lyricsAnalyzerDbContext.LyricsAnalyzerItems.ToArrayAsync();

        foreach (LyricsAnalyzerItem lyricsAnalyzerItem in lyricsAnalyzerItemsCosmosAll)
        {
            muzakBotDbContext.LyricsAnalyzerItems.Add(lyricsAnalyzerItem);
        }

        await muzakBotDbContext.SaveChangesAsync();

        logger.LogInformation("Migrated LyricsAnalyzerItems from CosmosDB to Postgres.");
    }
}
