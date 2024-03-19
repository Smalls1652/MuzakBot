using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Source generation context for the OpenAI models.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
)]
[JsonSerializable(typeof(ChatMessage))]
[JsonSerializable(typeof(ChatMessage[]))]
[JsonSerializable(typeof(OpenAiChatCompletionRequest))]
[JsonSerializable(typeof(OpenAiChoice))]
[JsonSerializable(typeof(OpenAiChoice[]))]
[JsonSerializable(typeof(OpenAiUsage))]
[JsonSerializable(typeof(OpenAiChatCompletion))]
internal partial class OpenAiJsonContext : JsonSerializerContext
{}