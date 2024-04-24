using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class LyricsAnalyzerCommandModule
{
    /// <summary>
    /// Gets the share links for a song/album from a streaming service URL.
    /// </summary>
    /// <param name="url">The URL to a song/album on a streaming service.</param>
    /// <returns>The music entity item.</returns>
    private async Task<MusicEntityItem> GetMusicEntityItemAsync(string url)
    {
        MusicEntityItem? musicEntityItem = null;
        try
        {
            musicEntityItem = await _odesliService.GetShareLinksAsync(url);

            if (musicEntityItem is null)
            {
                throw new Exception("No share links found.");
            }
        }
        catch (Exception)
        {
            throw;
        }

        return musicEntityItem;
    }

    /// <summary>
    /// Gets the platform entity link for a music entity item.
    /// </summary>
    /// <param name="musicEntityItem">The music entity item.</param>
    /// <returns>The platform entity link.</returns>
    private PlatformEntityLink GetPlatformEntityLink(MusicEntityItem musicEntityItem)
    {
        PlatformEntityLink? platformEntityLink = null!;

        try
        {
            platformEntityLink = musicEntityItem.LinksByPlatform!["itunes"];
        }
        catch (Exception)
        {
            try
            {
                platformEntityLink = musicEntityItem.LinksByPlatform!["appleMusic"];
            }
            catch (Exception)
            {
                throw;
            }
        }

        return platformEntityLink;
    }
}
