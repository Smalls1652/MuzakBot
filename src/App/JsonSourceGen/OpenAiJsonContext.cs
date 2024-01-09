using MuzakBot.App.Models.OpenAi;

namespace MuzakBot.App;

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