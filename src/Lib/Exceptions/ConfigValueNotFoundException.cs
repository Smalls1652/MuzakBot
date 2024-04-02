namespace MuzakBot.Lib;

/// <summary>
/// Represents an exception that is thrown when a configuration value is not found.
/// </summary>
public sealed class ConfigValueNotFoundException : Exception
{
    public ConfigValueNotFoundException(string configValueName) : base($"The configuration value '{configValueName}' was not found.")
    {
    }

    public ConfigValueNotFoundException(string configValueName, Exception innerException) : base($"The configuration value '{configValueName}' was not found.", innerException)
    {
    }
}