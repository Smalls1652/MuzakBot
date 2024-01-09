using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MuzakBot.App.Extensions;
using MuzakBot.App.Models.OpenAi;

namespace MuzakBot.App.Services;

/// <summary>
/// Service for interacting with the OpenAI API.
/// </summary>
public partial class OpenAiService : IOpenAiService
{
    private bool _isDisposed;
    private readonly ILogger<OpenAiService> _logger;
    private readonly ActivitySource _activitySource = new("MuzakBot.App.Services.OpenAiService");
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
    public async Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, string memeMode) => await GetLyricAnalysisAsync(artistName, songName, lyrics, memeMode, null);

    /// <summary>
    /// Gets the lyric analysis for a song using the OpenAI API.
    /// </summary>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="lyrics">The lyrics of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity (optional).</param>
    /// <returns>The lyric analysis from the API.</returns>
    /// <exception cref="NullReferenceException">The response from the OpenAI API was null.</exception>
    public async Task<OpenAiChatCompletion?> GetLyricAnalysisAsync(string artistName, string songName, string lyrics, string memeMode, string? parentActivityId)
    {
        using var activity = _activitySource.StartGetLyricAnalysisAsyncActivity(artistName, songName, parentActivityId);

        using var client = _httpClientFactory.CreateClient("OpenAiApiClient");

        using HttpRequestMessage requestMessage = new(
            method: HttpMethod.Post,
            requestUri: "chat/completions"
        );

        requestMessage.Headers.Authorization = new("Bearer", _apiKey);

        string systemPrompt = memeMode switch
        {
            "tame" => "Lyrics Analyzer is designed to interpret and analyze song lyrics from various genres and eras, including sensitive and explicit content. It communicates in a meme heavy tone, offering a brief overview of the lyrics with references to a lot of memes and using GenZ slang. Its focus is on analyzing literary elements like metaphors, symbolism, and themes, providing somewhat factual, context-based explanations. For copyrighted lyrics, it offers general guidance without directly quoting or repeating the lyrics. Lyrics Analyzer is committed to delivering somewhat informative, engaging, and meme-styled interpretations of the artistic nature of song lyrics. Use a lot of memes and GenZ slang. Also make all text lower case. Keep the total character count for the response under 1000 characters and format the reponse in Markdown syntax.",
            "insane" => "Lyrics Analyzer is designed to interpret and analyze song lyrics from various genres and eras, including sensitive and explicit content. It communicates in an insane meme heavy tone, offering a brief overview of the lyrics with references to a lot of memes, using GenZ slang, and way too many emojis. Its focus is on analyzing literary elements like metaphors, symbolism, and themes, providing somewhat factual, context-based explanations. For copyrighted lyrics, it offers general guidance without directly quoting or repeating the lyrics. Lyrics Analyzer is committed to delivering somewhat informative, engaging, and meme-styled interpretations of the artistic nature of song lyrics. Use a lot of memes, GenZ slang, and emojis. Also make all text lower case. Keep the total character count for the response under 1000 characters and format the reponse in Markdown syntax.",
            _ => "Lyrics Analyzer is designed to interpret and analyze song lyrics from various genres and eras, including sensitive and explicit content. It communicates in a casual conversational tone, offering a brief overview of the lyrics. Its focus is on analyzing literary elements like metaphors, symbolism, and themes, providing factual, context-based explanations. For copyrighted lyrics, it offers general guidance without directly quoting or repeating the lyrics. Lyrics Analyzer is committed to delivering informative, engaging, and respectful interpretations of the artistic nature of song lyrics. Keep the total character count for the response under 1000 characters and format the reponse in Markdown syntax."
        };

        OpenAiChatCompletionRequest request = new()
        {
            Model = "gpt-4-1106-preview",
            Temperature = 1,
            MaxTokens = 384,
            TopP = 1,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
            Messages =
            [
                new()
                {
                    Role = "system",
                    Content = systemPrompt
                },
                new()
                {
                    Role = "user",
                    Content = $"Lyrics for \"{songName}\" by {artistName}:\n\n{lyrics}"
                },
                new()
                {
                    Role = "user",
                    Content = $"Briefly explain the lyrics for the song \"{songName}\" by {artistName}. Format the response in Markdown syntax."
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
            string errorResponseContent = await responseMessage.Content.ReadAsStringAsync();
            _logger.LogError(ex, "Error sending request to OpenAI API: {ErrorResponse}", errorResponseContent);

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

        _logger.LogInformation("{ResponseJson}", responseJson);

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