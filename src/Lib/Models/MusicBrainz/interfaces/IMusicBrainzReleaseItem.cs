namespace MuzakBot.Lib.Models.MusicBrainz;

public interface IMusicBrainzReleaseItem
{
    string Id { get; set; }
    string Title { get; set; }
    string? Barcode { get; set; }
    string? Date { get; set; }
}