namespace MuzakBot.App.Models.MusicBrainz;

/// <summary>
/// The type of an artist retrieved from MusicBrainz.
/// </summary>
public enum MusicBrainzArtistType
{
    /// <summary>
    /// The artist is a single person.
    /// </summary>
    Person,

    /// <summary>
    /// The artist is a group of people.
    /// </summary>
    Group,

    /// <summary>
    /// The artist is an orchestra.
    /// </summary>
    Orchestra,

    /// <summary>
    /// The artist is a choir.
    /// </summary>
    Choir,

    /// <summary>
    /// The artist is a character.
    /// </summary>
    Character,

    /// <summary>
    /// The artist is an other unspecified type.
    /// </summary>
    Other
}