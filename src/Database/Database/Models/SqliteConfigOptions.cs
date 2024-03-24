using System.Text.Json.Serialization;

namespace MuzakBot.Database.Models;

public class SqliteConfigOptions
{
    [JsonPropertyName("SQLITE_DB_DIR_PATH")]
    public string DbDirPath { get; set; } = null!;

    public void EnsureDatabaseDirectoryExists()
    {
        if (!Directory.Exists(DbDirPath))
        {
            Directory.CreateDirectory(DbDirPath);
        }
    }
}
