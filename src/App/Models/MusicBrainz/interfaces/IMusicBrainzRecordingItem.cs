namespace MuzakBot.App.Models.MusicBrainz;

public interface IMusicBrainzRecordingItem
{
    string Id { get; set; }
    string Title { get; set; }
    MusicBrainzReleaseItem[]? Releases { get; set; }
}