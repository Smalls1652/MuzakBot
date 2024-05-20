namespace MuzakBot.Lib.Models.Genius;

/// <summary>
/// Represents the primary artist of a song on Genius.
/// </summary>
public class GeniusArtist
{
    /// <summary>
    /// The name of the primary artist.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
