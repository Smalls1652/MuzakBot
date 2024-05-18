using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.Lib;

/// <summary>
/// Source generation context for the OpenAI models.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never
)]
[JsonSerializable(typeof(OpenAiErrorResponse))]
[JsonSerializable(typeof(OpenAiErrorData))]
internal partial class OpenAiJsonContext : JsonSerializerContext
{
}
