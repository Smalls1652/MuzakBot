namespace MuzakBot.Lib.Models.Itunes;

public interface IAlbumItem
{
    string WrapperType { get; set; }
    string CollectionType { get; set; }
    long ArtistId { get; set; }
    long CollectionId { get; set; }
    string ArtistName { get; set; }
    string CollectionName { get; set; }
    string? CollectionCensoredName { get; set; }
    string CollectionViewUrl { get; set; }
}
