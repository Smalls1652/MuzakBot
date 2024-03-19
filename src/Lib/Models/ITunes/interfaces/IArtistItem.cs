namespace MuzakBot.Lib.Models.Itunes;

public interface IArtistItem
{
    string WrapperType { get; set; }
    string ArtistType { get; set; }
    string ArtistName { get; set; }
    string ArtistLinkUrl { get; set; }
    long ArtistId { get; set; }
    long? AmgArtistId { get; set; }
    string PrimaryGenreName { get; set; }
    long PrimaryGenreId { get; set; }
}