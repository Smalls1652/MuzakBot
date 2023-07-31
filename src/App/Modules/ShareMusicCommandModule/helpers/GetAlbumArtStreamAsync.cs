using MuzakBot.App.Models.Odesli;

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

        responseMessage.EnsureSuccessStatusCode();

        return await responseMessage.Content.ReadAsStreamAsync();
    }
}