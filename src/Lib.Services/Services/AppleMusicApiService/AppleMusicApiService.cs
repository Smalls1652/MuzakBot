using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using MuzakBot.Lib.Models.AppleMusic;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service for interacting with the Apple Music API.
/// </summary>
public partial class AppleMusicApiService : IAppleMusicApiService
{
    private string? _bearerToken;
    private bool _tokenIsBeingRefreshed = false;

    private readonly AppleMusicApiServiceOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleMusicApiService"/> class.
    /// </summary>
    /// <param name="options">The options for the service.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="logger">The logger.</param>
    public AppleMusicApiService(IOptions<AppleMusicApiServiceOptions> options, IHttpClientFactory httpClientFactory, ILogger<AppleMusicApiService> logger)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Search for artists on Apple Music.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>An array of artists.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs deserializing the search response.</exception>
    public async Task<Artist[]> SearchArtistsAsync(string searchTerm)
    {
        SearchRequest searchRequest = new(
            storefront: "us",
            term: searchTerm,
            searchTypes: [
                SearchType.Artists
            ],
            limit: 5
        );

        try
        {
            string responseContent = await SendRequestAsync(searchRequest);

            SearchResponse searchResponse = JsonSerializer.Deserialize(
                json: responseContent,
                jsonTypeInfo: AppleMusicApiJsonContext.Default.SearchResponse
            ) ?? throw new InvalidOperationException("Error deserializing search response.");

            if (searchResponse.Results.Artists is null)
            {
                throw new NullReferenceException("No artists found in search response.");
            }

            return searchResponse.Results.Artists.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for artists.");
            throw;
        }
    }

    /// <summary>
    /// Search for albums on Apple Music.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>An array of albums.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs deserializing the search response.</exception>
    public async Task<Album[]> SearchAlbumsAsync(string searchTerm)
    {
        SearchRequest searchRequest = new(
            storefront: "us",
            term: searchTerm,
            searchTypes: [
                SearchType.Albums
            ],
            limit: 5
        );

        try
        {
            string responseContent = await SendRequestAsync(searchRequest);

            SearchResponse searchResponse = JsonSerializer.Deserialize(
                json: responseContent,
                jsonTypeInfo: AppleMusicApiJsonContext.Default.SearchResponse
            ) ?? throw new InvalidOperationException("Error deserializing search response.");

            if (searchResponse.Results.Albums is null)
            {
                throw new NullReferenceException("No albums found in search response.");
            }

            return searchResponse.Results.Albums.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for albums.");
            throw;
        }
    }

    /// <summary>
    /// Search for songs on Apple Music.
    /// </summary>
    /// <param name="searchTerm">The search term.</param>
    /// <returns>An array of songs.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs deserializing the search response.</exception>
    public async Task<Song[]> SearchSongsAsync(string searchTerm)
    {
        SearchRequest searchRequest = new(
            storefront: "us",
            term: searchTerm,
            searchTypes: [
                SearchType.Songs
            ],
            limit: 5
        );

        try
        {
            string responseContent = await SendRequestAsync(searchRequest);

            SearchResponse searchResponse = JsonSerializer.Deserialize(
                json: responseContent,
                jsonTypeInfo: AppleMusicApiJsonContext.Default.SearchResponse
            ) ?? throw new InvalidOperationException("Error deserializing search response.");

            if (searchResponse.Results.Songs is null)
            {
                throw new NullReferenceException("No songs found in search response.");
            }

            return searchResponse.Results.Songs.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for songs.");
            throw;
        }
    }

    /// <summary>
    /// Get an artist from the Apple Music catalog.
    /// </summary>
    /// <param name="id">The ID of the artist.</param>
    /// <returns>The artist.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs deserializing the artist response.</exception>
    /// <exception cref="NullReferenceException">Thrown when no artist is found in the catalog response.</exception>
    public async Task<Artist> GetArtistFromCatalogAsync(string id)
    {
        CatalogRequest catalogRequest = new(
            storefront: "us",
            id: id,
            itemType: CatalogItemType.Artists
        );

        try
        {
            string responseContent = await SendRequestAsync(catalogRequest);

            AppleMusicResponse<Artist> artistResponse = JsonSerializer.Deserialize(
                json: responseContent,
                jsonTypeInfo: AppleMusicApiJsonContext.Default.AppleMusicResponseArtist
            ) ?? throw new InvalidOperationException("Error deserializing artist response.");

            if (artistResponse.Data is null || artistResponse.Data.Length == 0)
            {
                throw new NullReferenceException("No artist found in catalog response.");
            }

            return artistResponse.Data[0];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting artist from catalog.");
            throw;
        }
    }

    /// <summary>
    /// Get an album from the Apple Music catalog.
    /// </summary>
    /// <param name="id">The ID of the album.</param>
    /// <returns>The album.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs deserializing the album response.</exception>
    /// <exception cref="NullReferenceException">Thrown when no album is found in the catalog response.</exception>
    public async Task<Album> GetAlbumFromCatalogAsync(string id)
    {
        CatalogRequest catalogRequest = new(
            storefront: "us",
            id: id,
            itemType: CatalogItemType.Albums
        );

        try
        {
            string responseContent = await SendRequestAsync(catalogRequest);

            AppleMusicResponse<Album> artistResponse = JsonSerializer.Deserialize(
                json: responseContent,
                jsonTypeInfo: AppleMusicApiJsonContext.Default.AppleMusicResponseAlbum
            ) ?? throw new InvalidOperationException("Error deserializing artist response.");

            if (artistResponse.Data is null || artistResponse.Data.Length == 0)
            {
                throw new NullReferenceException("No album found in catalog response.");
            }

            return artistResponse.Data[0];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting album from catalog.");
            throw;
        }
    }

    /// <summary>
    /// Get a song from the Apple Music catalog.
    /// </summary>
    /// <param name="id">The ID of the song.</param>
    /// <returns>The song.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs deserializing the song response.</exception>
    /// <exception cref="NullReferenceException">Thrown when no song is found in the catalog response.</exception>
    public async Task<Song> GetSongFromCatalogAsync(string id)
    {
        CatalogRequest catalogRequest = new(
            storefront: "us",
            id: id,
            itemType: CatalogItemType.Songs
        );

        try
        {
            string responseContent = await SendRequestAsync(catalogRequest);

            AppleMusicResponse<Song> artistResponse = JsonSerializer.Deserialize(
                json: responseContent,
                jsonTypeInfo: AppleMusicApiJsonContext.Default.AppleMusicResponseSong
            ) ?? throw new InvalidOperationException("Error deserializing artist response.");

            if (artistResponse.Data is null || artistResponse.Data.Length == 0)
            {
                throw new NullReferenceException("No song found in catalog response.");
            }

            return artistResponse.Data[0];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting song from catalog.");
            throw;
        }
    }

    /// <summary>
    /// Send a request to the Apple Music API.
    /// </summary>
    /// <param name="searchRequest">The search request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the Apple Music API.</returns>
    private async Task<string> SendRequestAsync(IAppleMusicRequest request)
    {
        if (_tokenIsBeingRefreshed)
        {
            _logger.LogWarning("Token is being refreshed. Waiting for refresh to complete.");
            while (_tokenIsBeingRefreshed)
            {
                await Task.Delay(1000);
            }
        }

        if (_bearerToken is null || string.IsNullOrWhiteSpace(_bearerToken) || IsTokenExpired())
        {
            GenerateBearerToken();
        }

        _logger.LogInformation("Sending request to: {url}", request.CreateUrlPath());

        using HttpClient client = _httpClientFactory.CreateClient("AppleMusicApiClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: request.CreateUrlPath()
        );

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

        using HttpResponseMessage response = await client.SendAsync(requestMessage);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Generate a bearer token for the Apple Music API.
    /// </summary>
    private void GenerateBearerToken()
    {
        if (!IsTokenExpired())
        {
            return;
        }

        _tokenIsBeingRefreshed = true;

        _logger.LogInformation("Generating bearer token.");
        try
        {
            using ECDsa appKey = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            appKey.ImportPkcs8PrivateKey(_options.ConvertAppKeyToByteArray(), out _);

            ECDsaSecurityKey securityKey = new(appKey)
            {
                KeyId = _options.AppleAppKeyId
            };

            SigningCredentials signingCredentials = new(
                key: securityKey,
                algorithm: SecurityAlgorithms.EcdsaSha256
            );

            JsonWebTokenHandler tokenHandler = new();

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = _options.AppleTeamId,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(_options.TokenExpiration.TotalMinutes),
                SigningCredentials = signingCredentials
            };

            string token = tokenHandler.CreateToken(tokenDescriptor);

            _bearerToken = token;
        }
        finally
        {
            _tokenIsBeingRefreshed = false;
        }
    }

    /// <summary>
    /// Check if the token is expired.
    /// </summary>
    /// <returns>True if the token is expired, false otherwise.</returns>
    private bool IsTokenExpired()
    {
        if (_bearerToken is null || string.IsNullOrWhiteSpace(_bearerToken))
        {
            return true;
        }

        JsonWebTokenHandler tokenHandler = new();

        try
        {
            JsonWebToken jwt = tokenHandler.ReadJsonWebToken(_bearerToken);
            bool isExpired = jwt.ValidTo < DateTime.UtcNow;

            if (isExpired)
            {
                _logger.LogWarning("Token is expired.");
            }

            return isExpired;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading JWT");
            return true;
        }
    }
}
