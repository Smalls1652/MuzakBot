using System.ComponentModel.DataAnnotations.Schema;

namespace MuzakBot.Lib.Models.Database;

[Table("database_updates")]
public sealed class DatabaseUpdate : DatabaseItem
{
    [JsonConstructor]
    public DatabaseUpdate()
    {}

    [NotMapped]
    [JsonIgnore]
    public bool FirstRun { get; set; }

    [Column("migratedToEfCore")]
    [JsonPropertyName("migratedToEfCore")]
    public bool MigratedToEfCore { get; set; }
}
