using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
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
            /*
                Temporary fix:

                Amazon is being excluded from the fallback for the time being,
                because the "apiProvider" value doesn't cleanly match it's platform
                entity link key.
            */
            var streamingEntityWithThumbnailUrl = musicEntityItem.EntitiesByUniqueId!
                .FirstOrDefault(
                    entity => entity.Value.ThumbnailUrl is not null && entity.Value.ApiProvider != "amazon"
                )
                .Value
                .ApiProvider;

            if (!string.IsNullOrEmpty(streamingEntityWithThumbnailUrl))
            {
                platformEntityLink = musicEntityItem.LinksByPlatform![streamingEntityWithThumbnailUrl];
            }
            else
            {
                throw;
            }
        }

        return platformEntityLink;
    }
}
