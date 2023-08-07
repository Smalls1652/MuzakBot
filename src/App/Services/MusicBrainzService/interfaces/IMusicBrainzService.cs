using MuzakBot.App.Models.MusicBrainz;

namespace MuzakBot.App.Services;

public interface IMusicBrainzService
{
    Task<MusicBrainzArtistSearchResult?> SearchArtistAsync(string artistName);
}