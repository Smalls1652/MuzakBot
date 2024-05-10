using System.Text.RegularExpressions;

namespace MuzakBot.Lib.Services;

/// <summary>
/// Options for configuring <see cref="AppleMusicApiService"/>.
/// </summary>
public partial class AppleMusicApiServiceOptions
{
    /// <summary>
    /// The Apple Developer team ID.
    /// </summary>
    public string AppleTeamId { get; set; } = null!;

    /// <summary>
    /// The app ID for the app.
    /// </summary>
    public string AppleAppId { get; set; } = null!;

    /// <summary>
    /// The key for the app.
    /// </summary>
    public string AppleAppKey { get; set; } = null!;

    /// <summary>
    /// The ID for the key.
    /// </summary>
    public string AppleAppKeyId { get; set; } = null!;

    /// <summary>
    /// The user token for delegating requests.
    /// </summary>
    public string? AppleMusicUserToken { get; set; }

    /// <summary>
    /// The expiration time for the token.
    /// </summary>
    public TimeSpan TokenExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Convert the app key to a byte array.
    /// </summary>
    /// <returns>The app key as a byte array.</returns>
    public byte[] ConvertAppKeyToByteArray()
    {
        Match match = AppKeyRegex().Match(AppleAppKey);

        if (!match.Success)
        {
            throw new InvalidOperationException("Error parsing app key.");
        }

        string[] appKey = match
            .Groups["keyValue"]
            .Value
            .Split("\n");

        return Convert.FromBase64String(string.Join("", appKey));
    }

    [GeneratedRegex(
        pattern: "-{5}BEGIN PRIVATE KEY-{5}\n(?'keyValue'(?s).+?)\n-{5}END PRIVATE KEY-{5}"
    )]
    private static partial Regex AppKeyRegex();
}