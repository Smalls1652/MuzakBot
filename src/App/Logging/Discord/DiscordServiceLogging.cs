using Microsoft.Extensions.Logging;

namespace MuzakBot.App.Logging.Discord;

/// <summary>
/// Source generated logging methods for the <see cref="Services.DiscordService"/> class.
/// </summary>
internal static partial class DiscordServiceLogging
{
    /// <summary>
    /// Logs that the Discord service is connecting.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Connecting to Discord..."
    )]
    public static partial void LogDiscordStartConnecting(this ILogger logger);

    /// <summary>
    /// Logs that the Discord service is being initialized.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Initializing Discord interaction service."
    )]
    public static partial void LogInitializeDiscordInteractionService(this ILogger logger);

    /// <summary>
    /// Logs that the Discord bot is being logged in.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Logging the bot into Discord."
    )]
    public static partial void LogDiscordBotLogin(this ILogger logger);

    /// <summary>
    /// Logs that the Discord bot is being disconnected.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Disconnecting the bot from Discord..."
    )]
    public static partial void LogDiscordBotDisconnect(this ILogger logger);

    /// <summary>
    /// Logs that the Discord bot has been disconnected.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "The bot has been disconnected from Discord."
    )]
    public static partial void LogDiscordBotDisconnected(this ILogger logger);

    /// <summary>
    /// Logs that a core config value is missing for the service.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="errorMessage">The error message to log.</param>
    /// <param name="exception">The exception to log.</param>
    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "Missing config value: {errorMessage}"
    )]
    public static partial void LogMissingConfigValueError(this ILogger logger, string errorMessage, Exception? exception);

    /// <summary>
    /// Logs that the Discord service is being initialized.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="moduleName">The name of module that is being added to the Discord interaction service.</param>
    [LoggerMessage(
        EventId = 10,
        Level = LogLevel.Information,
        Message = "Adding '{moduleName}' to the Discord interaction service."
    )]
    public static partial void LogAddModuleToInteractionService(this ILogger logger, string moduleName);

    /// <summary>
    /// Logs that app commands are being registered to a test guild.
    /// </summary>
    /// <remarks>
    /// This is only logged when the bot is running in debug mode.
    /// </remarks>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="guildId">The ID of the guild that the app commands are being registered to.</param>
    [LoggerMessage(
        EventId = 21,
        Level = LogLevel.Information,
        Message = "Running in debug mode. Registering app commands to test guild '{guildId}'."
    )]
    public static partial void LogRegisterCommandsDebugMode(this ILogger logger, ulong guildId);

    /// <summary>
    /// Logs that app commands are being registered globally.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    [LoggerMessage(
        EventId = 20,
        Level = LogLevel.Information,
        Message = "Registering app commands globally."
    )]
    public static partial void LogRegisterCommandsGlobally(this ILogger logger);

    /// <summary>
    /// Logs that admin app commands are being registered to a guild.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="guildId">The ID of the guild that the admin app commands are being registered to.</param>
    [LoggerMessage(
        EventId = 22,
        Level = LogLevel.Information,
        Message = "Registering admin app commands to guild '{guildId}'."
    )]
    public static partial void LogRegisterAdminCommands(this ILogger logger, ulong guildId);

    /// <summary>
    /// Logs the slash commands that were loaded.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="slashCommands">The slash commands that were loaded.</param>
    [LoggerMessage(
        EventId = 23,
        Level = LogLevel.Information,
        Message = "Slash commands loaded: {slashCommands}"
    )]
    public static partial void LogSlashCommandsLoaded(this ILogger logger, string slashCommands);

    /// <summary>
    /// Logs that an interaction was successfully executed.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="userName">The name of the user that executed the interaction.</param>
    /// <param name="isDmInteraction">Whether or not the interaction was executed in a DM.</param>
    [LoggerMessage(
        EventId = 30,
        Level = LogLevel.Information,
        Message = "Successfully executed interaction that was executed by user '{userName}'. Is DM interaction: {isDmInteraction}."
    )]
    public static partial void LogSuccessfullyExecutedInteraction(this ILogger logger, string userName, bool isDmInteraction);

    /// <summary>
    /// Logs that an interaction failed to execute.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="userName">The name of the user that executed the interaction.</param>
    /// <param name="isDmInteraction">Whether or not the interaction was executed in a DM.</param>
    [LoggerMessage(
        EventId = 31,
        Level = LogLevel.Error,
        Message = "Failed to execute interaction that was executed by user '{userName}'. Is DM interaction: {isDmInteraction}."
    )]
    public static partial void LogFailedToExecuteInteraction(this ILogger logger, string userName, bool isDmInteraction);

    /// <summary>
    /// Logs the error message of an interaction that failed to execute.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="errorMessage">The error message returned by the interaction.</param>
    [LoggerMessage(
        EventId = 31,
        Level = LogLevel.Error,
        Message = "Interaction error message: {errorMessage}"
    )]
    public static partial void LogInteractionErrorMessage(this ILogger logger, string errorMessage);

    /// <summary>
    /// Logs that a slash command was successfully executed.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="userName">The name of the user that executed the slash command.</param>
    /// <param name="isDmInteraction">Whether or not the slash command was executed in a DM.</param>
    [LoggerMessage(
        EventId = 40,
        Level = LogLevel.Information,
        Message = "Successfully executed slash command that was executed by user '{userName}'. Is DM interaction: {isDmInteraction}."
    )]
    public static partial void LogSuccessfullyExecutedSlashCommand(this ILogger logger, string userName, bool isDmInteraction);

    /// <summary>
    /// Logs that a slash command failed to execute.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="userName">The name of the user that executed the slash command.</param>
    /// <param name="isDmInteraction">Whether or not the slash command was executed in a DM.</param>
    [LoggerMessage(
        EventId = 31,
        Level = LogLevel.Error,
        Message = "Failed to execute slash command that was executed by user '{userName}'. Is DM interaction: {isDmInteraction}."
    )]
    public static partial void LogFailedToExecuteSlashCommand(this ILogger logger, string userName, bool isDmInteraction);

    /// <summary>
    /// Logs the error message of a slash command that failed to execute.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="errorMessage">The error message returned by the interaction.</param>
    [LoggerMessage(
        EventId = 31,
        Level = LogLevel.Error,
        Message = "Slash command error message: {errorMessage}"
    )]
    public static partial void LogSlashCommandErrorMessage(this ILogger logger, string errorMessage);

    /// <summary>
    /// Logs that an autocomplete was successfully executed.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="userName">The name of the user that executed the autocomplete.</param>
    /// <param name="isDmInteraction">Whether or not the autocomplete was executed in a DM.</param>
    [LoggerMessage(
        EventId = 50,
        Level = LogLevel.Information,
        Message = "Successfully executed autocomplete that was executed by user '{userName}'. Is DM interaction: {isDmInteraction}."
    )]
    public static partial void LogSuccessfullyExecutedAutocomplete(this ILogger logger, string userName, bool isDmInteraction);

    /// <summary>
    /// Logs that an autocomplete failed to execute.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="userName">The name of the user that executed the autocomplete.</param>
    /// <param name="isDmInteraction">Whether or not the autocomplete was executed in a DM.</param>
    [LoggerMessage(
        EventId = 51,
        Level = LogLevel.Error,
        Message = "Failed to execute autocomplete that was executed by user '{userName}'. Is DM interaction: {isDmInteraction}."
    )]
    public static partial void LogFailedToExecuteAutocomplete(this ILogger logger, string userName, bool isDmInteraction);

    /// <summary>
    /// Logs the error message of an autocomplete that failed to execute.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> instance.</param>
    /// <param name="errorMessage">The error message returned by the interaction.</param>
    [LoggerMessage(
        EventId = 51,
        Level = LogLevel.Error,
        Message = "Autocomplete error message: {errorMessage}"
    )]
    public static partial void LogAutocompleteErrorMessage(this ILogger logger, string errorMessage);
}
