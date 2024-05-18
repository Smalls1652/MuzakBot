namespace MuzakBot.Lib;

/// <summary>
/// Holds data for an error that occurred while interacting with the OpenAI API.
/// </summary>
public sealed class OpenAiErrorData
{
    /// <summary>
    /// The error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// The type of error that occurred.
    /// </summary>
    [JsonPropertyName("type")]
    public string? ErrorType { get; set;}

    /// <summary>
    /// The parameter that caused the error.
    /// </summary>
    [JsonPropertyName("param")]
    public string? Param { get; set; }

    /// <summary>
    /// The code of the error.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}
