using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Logging.CosmosDb;

/// <summary>
/// Source generated logging methods for the <see cref="Services.CosmosDbService"/> class.
/// </summary>
internal static partial class CosmosDbServiceLogging
{
    /// <summary>
    /// Logs the start of an add or update operation to the database.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being added or updated.</param>
    /// <param name="id">A unique identifier for the item.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.AddOrUpdateOperation.Start",
        Level = LogLevel.Information,
        Message = "Adding or updating {itemType} for '{id}' to the database."
    )]
    public static partial void LogAddOrUpdateOperationStart(this ILogger logger, string itemType, string id);

    /// <summary>
    /// Logs the addition/update of an item to the database.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being added or updated.</param>
    /// <param name="id">A unique identifier for the item.</param>
    /// <param name="state">The type of operation.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.AddOrUpdateOperation.Added",
        Level = LogLevel.Information,
        Message = "{itemType} {state} for '{id}'."
    )]
    public static partial void LogAddOrUpdateOperationAdded(this ILogger logger, string itemType, string id, string state);

    /// <summary>
    /// Logs the start of a get operation to the database.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being retrieved.</param>
    /// <param name="id">A unique identifier for the item.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.GetOperation.Start",
        Level = LogLevel.Information,
        Message = "Getting {itemType} for '{id}' from the database."
    )]
    public static partial void LogGetOperationStart(this ILogger logger, string itemType, string id);

    /// <summary>
    /// Logs the retrieval of an item from the database.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being retrieved.</param>
    /// <param name="id">A unique identifier for the item.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.GetOperation.Found",
        Level = LogLevel.Information,
        Message = "{itemType} found for '{id}'."
    )]
    public static partial void LogGetOperationFound(this ILogger logger, string itemType, string id);

    /// <summary>
    /// Logs that an item was not found in the database.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being retrieved.</param>
    /// <param name="id">A unique identifier for the item.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.GetOperation.NotFound",
        Level = LogLevel.Warning,
        Message = "{itemType} not found for '{id}'."
    )]
    public static partial void LogGetOperationNotFound(this ILogger logger, string itemType, string id);

    /// <summary>
    /// Logs that the retrieval of an item from the database failed.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being retrieved.</param>
    /// <param name="id">A unique identifier for the item.</param>
    /// <param name="exception">The exception that occurred.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.GetOperation.Failed",
        Level = LogLevel.Error,
        Message = "Failed to get {itemType} for '{id}'."
    )]
    public static partial void LogGetOperationFailed(this ILogger logger, string itemType, string id, Exception? exception);

    /// <summary>
    /// Logs the initial creation of an item in the database during a get operation.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="itemType">The type of item being retrieved.</param>
    /// <param name="id">A unique identifier for the item.</param>
    [LoggerMessage(
        EventName = "CosmosDbService.GetOperation.InitialCreation",
        Level = LogLevel.Information,
        Message = "Creating {itemType} for '{id}' in the database."
    )]
    public static partial void LogGetOperationInitialCreation(this ILogger logger, string itemType, string id);
}