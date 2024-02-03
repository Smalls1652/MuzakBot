namespace MuzakBot.Lib.Models.CosmosDb;

/// <summary>
/// Interface for Azure CosmosDB API responses.
/// </summary>
/// <typeparam name="T">The type of the documents returned.</typeparam>
public interface ICosmosDbResponse<T>
{
    /// <summary>
    /// Results returned from the database.
    /// </summary>
    T[]? Documents { get; set; }

    /// <summary>
    /// The count of results returned.
    /// </summary>
    int Count { get; set; }
}