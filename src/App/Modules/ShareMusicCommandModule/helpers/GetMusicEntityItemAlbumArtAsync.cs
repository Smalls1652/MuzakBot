using MuzakBot.App.Models.Odesli;

namespace MuzakBot.App.Modules;

public partial class ShareMusicCommandModule
{
    private async Task<Stream> GetMusicEntityItemAlbumArtAsync(StreamingEntityItem entityItem)
    {
        var httpClient = _httpClientFactory.CreateClient("GenericClient");

        HttpResponseMessage responseMessage = await httpClient.GetAsync(entityItem.ThumbnailUrl);

        responseMessage.EnsureSuccessStatusCode();

        return await responseMessage.Content.ReadAsStreamAsync();
    }
}