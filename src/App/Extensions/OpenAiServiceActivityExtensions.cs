using System.Diagnostics;
using MuzakBot.App.Models.OpenAi;

namespace MuzakBot.App.Extensions;

public static class OpenAiServiceActivityExtensions
{
    public static Activity? StartGetLyricAnalysisAsyncActivity(this ActivitySource activitySource, string artistName, string songName) => StartGetLyricAnalysisAsyncActivity(activitySource, artistName, songName, null);
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