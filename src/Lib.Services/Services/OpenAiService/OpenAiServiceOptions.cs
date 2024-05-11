namespace MuzakBot.Lib.Services;

/// <summary>
/// Represents the options for the OpenAI service.
/// </summary>
public class OpenAiServiceOptions
{
    /// <summary>
    /// The API key used for authenticating API calls to the OpenAI API.
    /// </summary>
    public string ApiKey { get; set; } = null!;
}
