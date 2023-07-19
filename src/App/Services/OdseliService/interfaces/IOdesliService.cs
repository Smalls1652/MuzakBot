using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Services;

public interface IOdesliService
{
    Task<MusicEntityItem?> GetShareLinksAsync(string inputUrl);
}