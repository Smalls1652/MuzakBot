namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// The response to an Apple Music API request.
/// </summary>
/// <typeparam name="T">The type of data returned by the request.</typeparam>
public class AppleMusicResponse<T>
{
    /// <summary>
    /// The data returned by the request.
    /// </summary>
    [JsonPropertyName("data")]
    public T[]? Data { get; set; }
}