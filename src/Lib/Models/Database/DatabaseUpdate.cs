using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database;

/// <summary>
/// Holds data about updates to the database.
/// </summary>
[Table("database_updates")]
public sealed class DatabaseUpdate : DatabaseItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseUpdate"/> class.
    /// </summary>
    [JsonConstructor]
    public DatabaseUpdate()
    { }

    /// <summary>
    /// Whether this is the first time <see cref="DatabaseUpdate"/> is being added.
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    public bool FirstRun { get; set; }

    /// <summary>
    /// Whether the database has been fully migrated to EF Core.
    /// </summary>
    [Column("migratedToEfCore")]
    [JsonPropertyName("migratedToEfCore")]
    public bool MigratedToEfCore { get; set; }
}