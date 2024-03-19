namespace MuzakBot.Lib.Services;

/// <summary>
/// Represents the options for the Genius API service.
/// </summary>
public class GeniusApiServiceOptions
{
    /// <summary>
    /// The access token used for authenticating API calls to the Genius API.
    /// </summary>
    public string AccessToken { get; set; } = null!;
}