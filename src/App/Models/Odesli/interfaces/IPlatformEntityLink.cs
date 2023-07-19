namespace MuzakBot.App.Models.Odesli;

/// <summary>
/// Interface that defines properties for a music streaming platform's links to an entity.
/// </summary>
public interface IPlatformEntityLink
{
    /// <summary>
    /// The unqiue identifier used for looking up data returned by the music streaming platform.
    /// </summary>
    string? EntityUniqueId { get; set; }

    /// <summary>
    /// The URL to the music streaming platform's page for the entity.
    /// </summary>
    Uri? Url { get; set; }

    /// <summary>
    /// The URI to launch the mobile app of the music streaming platform.
    /// </summary>
    Uri? NativeAppUriMobile { get; set; }

    /// <summary>
    /// The URI to launch the desktop app of the music streaming platform.
    /// </summary>
    Uri? NativeAppUriDesktop { get; set; }
}