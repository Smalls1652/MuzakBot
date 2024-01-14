using System.Diagnostics;
using MuzakBot.App.Models.OpenAi;

namespace MuzakBot.App.Extensions;

/// <summary>
/// Extension method for <see cref="Services.OpenAiService"/> activity tracing.
/// </summary>
public static class OpenAiServiceActivityExtensions
{
    /// <summary>
    /// Starts an activity for <see cref="Services.OpenAiService.GetLyricAnalysisAsync(string, string, string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartGetLyricAnalysisAsyncActivity(this ActivitySource activitySource, string artistName, string songName) => StartGetLyricAnalysisAsyncActivity(activitySource, artistName, songName, null);

    /// <summary>
    /// Starts an activity for <see cref="Services.OpenAiService.GetLyricAnalysisAsync(string, string, string)"/>.
    /// </summary>
    /// <param name="activitySource">The activity source.</param>
    /// <param name="artistName">The name of the artist.</param>
    /// <param name="songName">The name of the song.</param>
    /// <param name="parentActivityId">The ID of the parent activity (optional).</param>
    /// <returns>The started activity.</returns>
    public static Activity? StartGetLyricAnalysisAsyncActivity(this ActivitySource activitySource, string artistName, string songName, string? parentActivityId)
    {
        return activitySource.StartActivity(
            name: "GetLyricAnalysisAsync",
            kind: ActivityKind.Internal,
            tags: new ActivityTagsCollection
            {
                { "artistName", artistName },
                { "songName", songName }
            },
            parentId: parentActivityId
        );
    }

    /// <summary>
    /// Adds tags to an OpenAI API activity for a chat completion request.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <param name="chatCompletionRequest">The <see cref="OpenAiChatCompletionRequest"/> used in the API call.</param>
    /// <returns>The modified activity.</returns>
    public static Activity? AddOpenAiChatCompletionRequestTags(this Activity? activity, OpenAiChatCompletionRequest chatCompletionRequest)
    {
        activity?.AddTag("openAI_request_model", chatCompletionRequest.Model);
        activity?.AddTag("openAI_request_temperature", chatCompletionRequest.Temperature);
        activity?.AddTag("openAI_request_maxTokens", chatCompletionRequest.MaxTokens);
        activity?.AddTag("openAI_request_topP", chatCompletionRequest.TopP);
        activity?.AddTag("openAI_request_frequencyPenalty", chatCompletionRequest.FrequencyPenalty);
        activity?.AddTag("openAI_request_presencePenalty", chatCompletionRequest.PresencePenalty);

        return activity;
    }

    /// <summary>
    /// Adds tags to an OpenAI API activity for a chat completion response.
    /// </summary>
    /// <param name="activity">The activity.</param>
    /// <param name="chatCompletionResponse">The <see cref="OpenAiChatCompletion"/> response from the API call.</param>
    /// <returns>The modified activity.</returns>
    public static Activity? AddOpenAiChatCompletionResponseTags(this Activity? activity, OpenAiChatCompletion? chatCompletionResponse)
    {
        if (activity is null)
        {
            return null;
        }

        if (chatCompletionResponse is not null && chatCompletionResponse.Usage is not null)
        {
            activity?.AddTag("openAI_response_promptTokens", chatCompletionResponse.Usage.PromptTokens);
            activity?.AddTag("openAI_response_completionTokens", chatCompletionResponse.Usage.CompletionTokens);
            activity?.AddTag("openAI_response_totalTokens", chatCompletionResponse.Usage.TotalTokens);
        }

        return activity;
    }
}