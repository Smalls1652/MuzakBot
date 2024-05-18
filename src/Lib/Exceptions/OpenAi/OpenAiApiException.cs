using System.Net;

using MuzakBot.Lib.Models.OpenAi;

namespace MuzakBot.Lib;

/// <summary>
/// Exception thrown when an error occurs while interacting with the OpenAI API.
/// </summary>
public class OpenAiApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAiApiException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorResponseContent">The content of the error response from OpenAI.</param>
    /// <param name="httpStatusCode">The HTTP status code of the response.</param>
    public OpenAiApiException(string message, string errorResponseContent, HttpStatusCode httpStatusCode) : base(message)
    {
        ApiErrorResponse = JsonSerializer.Deserialize(
            json: errorResponseContent,
            jsonTypeInfo: OpenAiJsonContext.Default.OpenAiErrorResponse
        );

        HttpStatusCode = httpStatusCode;
        HttpStatusCodeMessage = GetCommonHttpStatusCodeMessage(httpStatusCode);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAiApiException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorResponseContent">The content of the error response from OpenAI.</param>
    /// <param name="httpStatusCode">The HTTP status code of the response.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public OpenAiApiException(string message, string errorResponseContent, HttpStatusCode httpStatusCode, Exception innerException) : base(message, innerException)
    {
        ApiErrorResponse = JsonSerializer.Deserialize(
            json: errorResponseContent,
            jsonTypeInfo: OpenAiJsonContext.Default.OpenAiErrorResponse
        );

        HttpStatusCode = httpStatusCode;
        HttpStatusCodeMessage = GetCommonHttpStatusCodeMessage(httpStatusCode);
    }

    /// <summary>
    /// The error response from OpenAI.
    /// </summary>
    public OpenAiErrorResponse? ApiErrorResponse { get; set; }

    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    public HttpStatusCode HttpStatusCode { get; set; }

    /// <summary>
    /// A message that describes the HTTP status code.
    /// </summary>
    public string HttpStatusCodeMessage { get; set; }

    private static string GetCommonHttpStatusCodeMessage(HttpStatusCode httpStatusCode)
    {
        return httpStatusCode switch
        {
            HttpStatusCode.BadRequest => "Your request was malformed or missing some required parameters, such as a token or an input.",
            HttpStatusCode.InternalServerError => "An error occurred on the OpenAI server. Please try again later.",
            HttpStatusCode.NotFound => "The requested resource does not exist.",
            HttpStatusCode.Unauthorized => "The provided API key does not have permission to access the requested resource.",
            HttpStatusCode.TooManyRequests => "You have either exceeded the rate limit for the OpenAI API or you have run out of credits. Please try again later.",
            HttpStatusCode.ServiceUnavailable => "OpenAI's API is experiencing high traffic. Please try again later.",
            _ => "Unknown"
        };
    }
}
