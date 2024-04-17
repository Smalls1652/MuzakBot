using System.Reflection;

using Discord;

namespace MuzakBot.App.Models.Responses;

/// <summary>
/// Represents a response containing information about the bot.
/// </summary>
public sealed class BotInfoResponse : IResponse
{
    private readonly string _currentYear = DateTimeOffset.Now.ToString("yyyy");
    private readonly Version? _assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
    private readonly string _uptimeTimestamp = Environment.GetEnvironmentVariable("APP_START_TIME") is not null && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APP_START_TIME"))
        ? $"<t:{Environment.GetEnvironmentVariable("APP_START_TIME")}:R>"
        : "`N/A`";

    /// <summary>
    /// Initialize a new instance of the <see cref="BotInfoResponse"/> class.
    /// </summary>
    public BotInfoResponse()
    {
        BotName = "MuzakBot";
        BotLogoUrl = "https://raw.githubusercontent.com/Smalls1652/MuzakBot/main/.github/images/logo.png";
        SourceCodeUrl = "https://github.com/Smalls1652/MuzakBot";
        HelpDocsUrl = "https://github.com/Smalls1652/MuzakBot/wiki/Bot-usage";

        ResponseText = $"""
A Discord bot to easily share music from one music streaming service to other streaming services.
## Bot Info

* 🔢 **Version:** `{_assemblyVersion ?? new Version(0, 0, 0, 0)}`
* 🕒 **Uptime:** {_uptimeTimestamp}
""";
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="BotInfoResponse"/> class.
    /// </summary>
    /// <param name="botInviteUrl">URL to invite the bot to a server.</param>
    public BotInfoResponse(string? botInviteUrl) : this()
    {
        BotInviteUrl = botInviteUrl;
    }

    /// <summary>
    /// The name of the bot.
    /// </summary>
    public string BotName { get; }

    /// <summary>
    /// The URL to the bot's logo.
    /// </summary>
    public string BotLogoUrl { get; }

    /// <summary>
    /// The text of the response.
    /// </summary>
    public string ResponseText { get; }

    /// <summary>
    /// The URL to the bot's source code.
    /// </summary>
    public string SourceCodeUrl { get; }

    /// <summary>
    /// The URL to the bot's help documentation.
    /// </summary>
    public string HelpDocsUrl { get; }

    /// <summary>
    /// The URL to invite the bot to a server.
    /// </summary>
    public string? BotInviteUrl { get; }

    /// <inheritdoc />
    public string GenerateText() => throw new NotImplementedException("This method is not implemented for this response type.");

    /// <inheritdoc />
    public EmbedBuilder GenerateEmbed()
    {
        EmbedBuilder botInfoEmbedBuilder = new();

        botInfoEmbedBuilder
            .WithTitle(BotName)
            .WithDescription(ResponseText)
            .WithFooter($"© 2023-{_currentYear} Tim Small (Smalls.Online)", BotLogoUrl)
            .WithImageUrl(BotLogoUrl);

        return botInfoEmbedBuilder;
    }

    /// <inheritdoc />
    public ComponentBuilder GenerateComponent()
    {
        ComponentBuilder botInfoComponent = new();

        botInfoComponent
            .WithButton(
                label: "Source code",
                style: ButtonStyle.Link,
                url: SourceCodeUrl,
                row: 0
            )
            .WithButton(
                label: "How to use",
                style: ButtonStyle.Link,
                url: HelpDocsUrl,
                row: 0
            );

        if (BotInviteUrl is not null && !string.IsNullOrEmpty(BotInviteUrl))
        {
            botInfoComponent
                .WithButton(
                    label: "Add to server",
                    style: ButtonStyle.Link,
                    url: BotInviteUrl,
                    row: 1
                );
        }

        return botInfoComponent;
    }
}
