using System.Text.Json.Serialization;

namespace MuzakBot.Lib.Models.MusicBrainz;

/// <summary>
/// Represents a release search result from MusicBrainz.
/// </summary>
public class MusicBrainzReleaseSearchResult
{
    /// <summary>
    /// Gets or sets the date and time when the search result was created.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the total number of releases in the search result.
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the offset of the search result.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>
    /// Gets or sets the array of music releases in the search result.
    /// </summary>
    [JsonPropertyName("releases")]
    public MusicBrainzReleaseItem[]? Releases { get; set; }

    /// <summary>
    /// Gets an array of distinct music releases based on their titles.
    /// </summary>
    /// <returns>An array of distinct music releases.</returns>
    public MusicBrainzReleaseItem[]? GetDistinct()
    {
        return Releases?.DistinctBy(item => item.Title).ToArray();
    }
}