using System.Text.Json.Serialization;

namespace MuzakBot.App.Models.MusicBrainz;

/// <summary>
/// Represents a recording search result retrieved from MusicBrainz.
/// </summary>
public class MusicBrainzRecordingSearchResult
{
    /// <summary>
    /// Gets or sets the creation date and time of the search result.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the total count of recordings in the search result.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the offset value used for pagination in the search result.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the array of music recordings in the search result.
    /// </summary>
    [JsonPropertyName("recordings")]
    public MusicBrainzRecordingItem[]? Recordings { get; set; }

    /// <summary>
    /// Returns an array of distinct music recordings based on their titles.
    /// </summary>
    /// <returns>An array of distinct music recordings.</returns>
    public MusicBrainzRecordingItem[]? GetDistinct()
    {
        return Recordings?.DistinctBy(item => item.Title).ToArray();
    }
}