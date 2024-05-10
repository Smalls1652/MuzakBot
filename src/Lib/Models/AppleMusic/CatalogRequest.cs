using System.Text;

namespace MuzakBot.Lib.Models.AppleMusic;

/// <summary>
/// Data for constructing a catalog request to the Apple Music API.
/// </summary>
public sealed class CatalogRequest : IAppleMusicRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogRequest"/> class.
    /// </summary>
    /// <param name="storefront">The Apple Music storefront to search in.</param>
    /// <param name="id">The ID of the catalog item.</param>
    /// <param name="itemType">The type of catalog item.</param>
    public CatalogRequest(string storefront, string id, CatalogItemType itemType)
    {
        Storefront = storefront;
        Id = id;
        ItemType = itemType;
        _itemTypeString = CatalogItemTypeToString(itemType);
    }

    /// <summary>
    /// The Apple Music storefront to search in.
    /// </summary>
    /// <remarks>
    /// This is specified by an ISO 3166 alpha-2 country code.
    /// </remarks>
    public string Storefront { get; set; } = null!;

    /// <summary>
    /// The ID of the catalog item.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// The type of catalog item.
    /// </summary>
    public CatalogItemType ItemType { get; set; }

    /// <summary>
    /// The localization tag for the request.
    /// </summary>
    public string? Localization { get; set; }

    /// <summary>
    /// The <see cref="ItemType"/> as a string.
    /// </summary>
    private readonly string _itemTypeString;

    /// <summary>
    /// Create a URL path for the catalog request.
    /// </summary>
    /// <returns>A string representing the URL path.</returns>
    public string CreateUrlPath()
    {
        StringBuilder urlPathBuilder = new($"v1/catalog/{Storefront}/{_itemTypeString}/{Id}");

        // Add the localization to the query string if it is not null.
        if (Localization != null && !string.IsNullOrEmpty(Localization))
        {
            urlPathBuilder.Append($"?l={Localization}");
        }

        return urlPathBuilder.ToString();
    }

    /// <summary>
    /// Converts a <see cref="CatalogItemType"/> to a string.
    /// </summary>
    /// <param name="itemType">The catalog item type.</param>
    /// <returns>The catalog item type as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when the catalog item type is invalid.</exception>
    private static string CatalogItemTypeToString(CatalogItemType itemType)
    {
        return itemType switch
        {
            CatalogItemType.Albums => "albums",
            CatalogItemType.Artists => "artists",
            CatalogItemType.Songs => "songs",
            _ => throw new ArgumentException("Invalid catalog item type.", nameof(itemType))
        };
    }
}