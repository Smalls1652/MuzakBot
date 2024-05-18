namespace MuzakBot.Lib.Models.OpenAi;

/// <summary>
/// The response from the OpenAI API when an error occurs.
/// </summary>
public sealed class OpenAiErrorResponse
{
    /// <summary>
    /// The error message.
    /// </summary>
    [JsonPropertyName("error")]
    public OpenAiErrorData Error { get; set; } = null!;
}
