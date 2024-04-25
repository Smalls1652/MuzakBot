using Microsoft.AspNetCore.Components;

namespace MuzakBot.WebApp.Components.Core;

public partial class PageCard : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}