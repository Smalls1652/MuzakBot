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
            limit: 10
        );

        try
        {
            using HttpResponseMessage response = await SendRequestAsync(searchRequest);

            SearchResponse<Artist> searchResponse = await JsonSerializer.DeserializeAsync(
                utf8Json: await response.Content.ReadAsStreamAsync(),
                jsonTypeInfo: AppleMusicApiJsonContext.Default.SearchResponseArtist
            ) ?? throw new InvalidOperationException("Error deserializing search response.");

            return searchResponse.Results.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for artists.");
            throw;
        }
    }

    /// <summary>
    /// Send a request to the Apple Music API.
    /// </summary>
    /// <param name="searchRequest">The search request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the Apple Music API.</returns>
    private async Task<HttpResponseMessage> SendRequestAsync(SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        if (_tokenIsBeingRefreshed)
        {
            _logger.LogWarning("Token is being refreshed. Waiting for refresh to complete.");
            while (_tokenIsBeingRefreshed)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }

        if (_bearerToken is null || string.IsNullOrWhiteSpace(_bearerToken) || IsTokenExpired())
        {
            GenerateBearerToken();
        }

        using HttpClient client = _httpClientFactory.CreateClient("AppleMusicApiClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: searchRequest.CreateUrlPath()
        );

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);

        HttpResponseMessage response = await client.SendAsync(requestMessage, cancellationToken);

        response.EnsureSuccessStatusCode();

        return response;
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
            using CngKey privateKey = CngKey.Import(
                keyBlob: _options.ConvertAppKeyToByteArray(),
                format: CngKeyBlobFormat.Pkcs8PrivateBlob
            );

            using ECDsaCng appKey = new(privateKey)
            {
                HashAlgorithm = CngAlgorithm.Sha256
            };

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
