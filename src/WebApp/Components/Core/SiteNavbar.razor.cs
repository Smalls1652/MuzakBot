using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Components;

namespace MuzakBot.WebApp.Components.Core;

public partial class SiteNavbar : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    private string? _currentPath;
    private string? _currentTopLevelPage;
    private string? _currentSecondaryPages;

    protected override void OnInitialized()
    {
        _currentPath = ExtractUriSections(NavigationManager.Uri).Groups["path"].Value;
        _currentTopLevelPage = ExtractUriSections(NavigationManager.Uri).Groups["topLevelPage"].Value;
        _currentSecondaryPages = ExtractUriSections(NavigationManager.Uri).Groups["secondaryPages"].Value;
    }

    [GeneratedRegex(
        pattern: "^(?:https|http)://(?'hostName'.+?)(?'path'/(?'topLevelPage'.*?)(?'secondaryPages'/.*?|))(?:#.*|)$",
        options: RegexOptions.Multiline
    )]
    private static partial Regex UriSectionRegex();

    private Match ExtractUriSections(string url)
    {
        Match uriSectionMatch = UriSectionRegex().Match(url);

        if (uriSectionMatch.Success == false)
        {
            throw new($"Failed to parse the current page to update the navigation bar. Uri provided: {url}");
        }

        return uriSectionMatch;
    }
}