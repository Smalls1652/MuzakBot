namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Holds metadata returned by the Genius API.
/// </summary>
public class GeniusMeta : IGeniusMeta
{
    /// <summary>
    /// The HTTP status code returned by the API.
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}
