namespace MuzakBot.App.Services;

/// <summary>
/// Represents the options for the Cosmos DB service.
/// </summary>
public class CosmosDbServiceOptions
{
    /// <summary>
    /// The connection string to use for connecting to the Cosmos DB instance.
    /// </summary>
    public string ConnectionString { get; set; } = null!;

    /// <summary>
    /// The name of the database to use.
    /// </summary>
    public string DatabaseName { get; set; } = null!;
}