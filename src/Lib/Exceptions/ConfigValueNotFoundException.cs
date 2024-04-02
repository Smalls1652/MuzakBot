namespace MuzakBot.Lib;

/// <summary>
/// Represents an exception that is thrown when a configuration value is not found.
/// </summary>
public sealed class ConfigValueNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="ConfigValueNotFoundException"/>.
    /// </summary>
    /// <param name="configValueName">The name of the config value.</param>
    public ConfigValueNotFoundException(string configValueName) : base($"The configuration value '{configValueName}' was not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ConfigValueNotFoundException"/>.
    /// </summary>
    /// <param name="configValueName">The name of the config value.</param>
    /// <param name="innerException">A reference to an exception that is the cause of this exception.</param>
    public ConfigValueNotFoundException(string configValueName, Exception innerException) : base($"The configuration value '{configValueName}' was not found.", innerException)
    {
    }
}