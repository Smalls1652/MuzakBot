namespace MuzakBot.Lib.Models.MusicBrainz;

public interface IMusicBrainzArtistItem
{
    string Id { get; set; }
    string Name { get; set; }
    string SortName { get; set; }
    string? Disambiguation { get; set; }
    string? Country { get; set; }
    string? Type { get; set; }
}
