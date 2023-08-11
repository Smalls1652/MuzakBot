namespace MuzakBot.App.Models.Itunes;

public interface ISongItem
{
    string WrapperType { get; set; }
    string Kind { get; set; }
    long ArtistId { get; set; }
    long CollectionId { get; set; }
    long TrackId { get; set; }
    string ArtistName { get; set; }
    string CollectionName { get; set; }
    string TrackName { get; set; }
    string CollectionCensoredName { get; set; }
    string TrackCensoredName { get; set; }
    string ArtistViewUrl { get; set; }
    string CollectionViewUrl { get; set; }
    string TrackViewUrl { get; set; }
    string PreviewUrl { get; set; }
    string ArtworkUrl30 { get; set; }
    string ArtworkUrl60 { get; set; }
    string ArtworkUrl100 { get; set; }
    double CollectionPrice { get; set; }
    double TrackPrice { get; set; }
    DateTimeOffset ReleaseDate { get; set; }
    string CollectionExplicitness { get; set; }
    string TrackExplicitness { get; set; }
    int DiscCount { get; set; }
    int DiscNumber { get; set; }
    int TrackCount { get; set; }
    int TrackNumber { get; set; }
    long TrackTimeMillis { get; set; }
    string Country { get; set; }
    string Currency { get; set; }
    string PrimaryGenreName { get; set; }
    bool IsStreamable { get; set; }
}