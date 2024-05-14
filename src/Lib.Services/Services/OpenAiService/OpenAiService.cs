using System.Diagnostics;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MuzakBot.Lib.Models.Database.LyricsAnalyzer;
using MuzakBot.Lib.Models.OpenAi;
using MuzakBot.Lib.Services.Extensions.Telemetry;
using MuzakBot.Lib.Services.Logging.OpenAi;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Service for interacting with the OpenAI API.
/// </summary>
public partial class OpenAiService : IOpenAiService
{
    private bool _isDisposed;
    private readonly ILogger<OpenAiService> _logger;
    private readonly ActivitySource _activitySource = new("MuzakBot.Lib.Services.OpenAiService");
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAiService"/> class.
    /// </summary>
    /// <param name="options">The <see cref="OpenAiServiceOptions"/> for configuring the service.</param>
    /// <param name="httpClientFactory">The <see cref="IHttpClientFactory"/>.</param>
    /// <param name="logger">The logger.</param>
    public OpenAiService(IOptions<OpenAiServiceOptions> options, IHttpClientFactory httpClientFactory, ILogger<OpenAiService> logger)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _apiKey = options.Value.ApiKey;
    }

    /// <summary>
    /// Gets the lyric analysis for a song using the OpenAI API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="lyrics">The lyrics of the song.</param>
    /// <returns>The lyric analysis from the API.</returns>
    /// <exception cref="NullReferenceException">The response from the OpenAI API was null.</exception>
    public async Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, LyricsAnalyzerPromptStyle promptStyle) => await GetLyricAnalysisAsync(artistName, songName, lyrics, promptStyle, null);

    /// <summary>
    /// Gets the lyric analysis for a song using the OpenAI API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="lyrics">The lyrics of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity (optional).</param>
    /// <returns>The lyric analysis from the API.</returns>
    /// <exception cref="NullReferenceException">The response from the OpenAI API was null.</exception>
    public async Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, LyricsAnalyzerPromptStyle promptStyle, string? parentActivityId)
    {
        using var activity = _activitySource.StartGetLyricAnalysisAsyncActivity(artistName, songName, parentActivityId);

        _logger.LogOpenAiApiServiceLyricAnalysisStart(artistName, songName, promptStyle.ShortName);

        using var client = _httpClientFactory.CreateClient("OpenAiApiClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Post,
            requestUri: "chat/completions"
        );

        requestMessage.Headers.Authorization = new("Bearer", _apiKey);

        OpenAiChatCompletionRequest request = new()
        {
            Model = "gpt-4o",
            Temperature = 1,
            MaxTokens = 512,
            TopP = 1,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
            Messages =
            [
                new()
                {
                    Role = "system",
                    Content = promptStyle.Prompt
                },
                new()
                {
                    Role = "user",
                    Content = $"Lyrics for \"{songName}\" by {artistName}:\n\n{lyrics}"
                },
                new()
                {
                    Role = "user",
                    Content = promptStyle.UserPrompt.Replace("{{songName}}", songName).Replace("{{artistName}}", artistName)
                }
            ]
        };

        activity?.AddOpenAiChatCompletionRequestTags(request);

        string requestBodyJson = JsonSerializer.Serialize(
            value: request,
            jsonTypeInfo: OpenAiJsonContext.Default.OpenAiChatCompletionRequest
        );

        requestMessage.Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");

        using HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

        try
        {
            responseMessage.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogOpenAiApiServiceFailure(ex);
            activity?.SetStatus(ActivityStatusCode.Error);

            throw;
        }

        Stream responseStream = await responseMessage.Content.ReadAsStreamAsync();

        OpenAiChatCompletion? response = await JsonSerializer.DeserializeAsync<OpenAiChatCompletion>(
            utf8Json: responseStream
        );

        if (response is null)
        {
            throw new NullReferenceException("The response from the OpenAI API was null.");
        }

        string responseJson = JsonSerializer.Serialize(
            value: response,
            jsonTypeInfo: OpenAiJsonContext.Default.OpenAiChatCompletion
        );

        activity?.AddOpenAiChatCompletionResponseTags(response);

        return response;
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _activitySource.Dispose();

        _isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
