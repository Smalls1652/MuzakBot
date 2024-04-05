using System.Text;
using System.Web;

namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Data for constructing a search request to the Apple Music API.
/// </summary>
public sealed class SearchRequest : IAppleMusicRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SearchRequest"/> class.
    /// </summary>
    /// <param name="storefront">The Apple Music storefront to search in.</param>
    /// <param name="term">The search term.</param>
    /// <param name="searchTypes">The types of resources to search for.</param>
    public SearchRequest(string storefront, string term, SearchType[] searchTypes)
    {
        Storefront = storefront;
        Term = term;
        Types = searchTypes;
        _searchTypesStrings = SearchTypesToString(searchTypes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchRequest"/> class.
    /// </summary>
    /// <param name="storefront">The Apple Music storefront to search in.</param>
    /// <param name="term">The search term.</param>
    /// <param name="searchTypes">The types of resources to search for.</param>
    /// <param name="limit">The number of resources to return.</param>
    /// <exception cref="ArgumentException">Thrown when the limit is not between 1 and 25.</exception>
    public SearchRequest(string storefront, string term, SearchType[] searchTypes, int limit) : this(storefront, term, searchTypes)
    {
        if (limit < 1 || limit > 25)
        {
            throw new ArgumentException("Limit must be between 1 and 25.", nameof(limit));
        }

        Limit = limit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchRequest"/> class.
    /// </summary>
    /// <param name="storefront">The Apple Music storefront to search in.</param>
    /// <param name="term">The search term.</param>
    /// <param name="searchTypes">The types of resources to search for.</param>
    /// <param name="limit">The number of resources to return.</param>
    /// <param name="offset">The next page of resources.</param>
    public SearchRequest(string storefront, string term, SearchType[] searchTypes, int limit, string? offset) : this(storefront, term, searchTypes, limit)
    {
        Offset = offset;
    }

    /// <summary>
    /// The Apple Music storefront to search in.
    /// </summary>
    /// <remarks>
    /// This is specified by an ISO 3166 alpha-2 country code.
    /// </remarks>
    public string Storefront { get; set; } = null!;

    /// <summary>
    /// The search term.
    /// </summary>
    public string Term { get; set; } = null!;

    /// <summary>
    /// The localization tag for the request.
    /// </summary>
    public string? Localization { get; set; }

    /// <summary>
    /// The number of resources to return.
    /// </summary>
    /// <remarks>
    /// The maximum value is 25.
    /// </remarks>
    public int Limit { get; set; } = 5;

    /// <summary>
    /// The next page of resources.
    /// </summary>
    public string? Offset { get; set; }

    /// <summary>
    /// The types of resources to search for.
    /// </summary>
    public SearchType[] Types { get; set; } = null!;

    /// <summary>
    /// Extra modifications for the request.
    /// </summary>
    public string[]? With { get; set; }

    /// <summary>
    /// The <see cref="Types"/> values as strings.
    /// </summary>
    private readonly string[] _searchTypesStrings;

    /// <summary>
    /// Create a URL path for the search request.
    /// </summary>
    /// <returns>A string representing the URL path.</returns>
    public string CreateUrlPath()
    {
        StringBuilder urlPathBuilder = new($"v1/catalog/{Storefront}/search");

        // Add the search types to the query string.
        urlPathBuilder.Append($"?types={string.Join(",", _searchTypesStrings)}");

        // Encode the search term and add it to the query string.
        string encodedTermString = HttpUtility.UrlEncode(Term);
        urlPathBuilder.Append($"&term={encodedTermString}");

        // Add the limit to the query string.
        urlPathBuilder.Append($"&limit={Limit}");

        // Add the offset to the query string if it is not null.
        if (Offset != null && !string.IsNullOrEmpty(Offset))
        {
            urlPathBuilder.Append($"&offset={Offset}");
        }

        // Add the localization to the query string if it is not null.
        if (Localization != null && !string.IsNullOrEmpty(Localization))
        {
            urlPathBuilder.Append($"&l={Localization}");
        }

        // Add the with modifications to the query string if it is not null.
        if (With != null && With.Length > 0)
        {
            urlPathBuilder.Append($"&with={string.Join(",", With)}");
        }

        return urlPathBuilder.ToString();
    }

    /// <summary>
    /// Convert an array of <see cref="SearchType"/> to an array of strings.
    /// </summary>
    /// <param name="searchTypes">The array of <see cref="SearchType"/> to convert.</param>
    /// <returns>An array of strings representing the search types.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid search type is encountered.</exception>
    private static string[] SearchTypesToString(SearchType[] searchTypes)
    {
        string[] searchTypesStrings = new string[searchTypes.Length];

        for (int i = 0; i < searchTypes.Length; i++)
        {
            searchTypesStrings[i] = searchTypes[i] switch
            {
                SearchType.Activities => "activities",
                SearchType.Albums => "albums",
                SearchType.AppleCurators => "apple-curators",
                SearchType.Artists => "artists",
                SearchType.Curators => "curators",
                SearchType.MusicVideos => "music-videos",
                SearchType.Playlists => "playlists",
                SearchType.RecordLabels => "record-labels",
                SearchType.Songs => "songs",
                SearchType.Stations => "stations",
                _ => throw new ArgumentException("Invalid search type.")
            };
        }

        return searchTypesStrings;
    }
}
