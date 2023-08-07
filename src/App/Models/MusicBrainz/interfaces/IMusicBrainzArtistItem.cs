namespace MuzakBot.App.Models.MusicBrainz;

public interface IMusicBrainzArtistItem
{
    string Id { get; set; }
    string Name { get; set; }
    string SortName { get; set; }
    string? Disambiguation { get; set; }
    string? Country { get; set; }
    MusicBrainzArtistType Type { get; set; }
}