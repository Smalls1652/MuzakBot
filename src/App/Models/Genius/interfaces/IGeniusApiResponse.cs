namespace MuzakBot.App.Models.Genius;

public interface IGeniusApiResponse<T>
{
    GeniusMeta Meta { get; set; }

    T? Response { get; set; }
}