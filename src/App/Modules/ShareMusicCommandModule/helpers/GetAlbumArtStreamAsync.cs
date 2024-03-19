using Microsoft.Extensions.Logging;
using MuzakBot.Lib.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    /// <summary>
    /// Gets the album art for the music item.
    /// </summary>
    /// <param name="entityItem">The streaming service data for the song/album.</param>
    /// <returns></returns>
    private async Task<Stream> GetAlbumArtStreamAsync(StreamingEntityItem entityItem)
    {
        var httpClient = _httpClientFactory.CreateClient("GenericClient");

        HttpResponseMessage responseMessage = await httpClient.GetAsync(entityItem.ThumbnailUrl);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Received an unsuccessful HTTP response for '{thumbnailUrl}'.", entityItem.ThumbnailUrl);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ran into an unknown issue while retrieving album artwork from '{thumbnailUrl}'.", entityItem.ThumbnailUrl);
            throw;
        }

        return await responseMessage.Content.ReadAsStreamAsync();
    }
}