namespace MuzakBot.App.Models.OpenAi;

public interface IChatMessage
{
    string Role { get; set; }
    string? Name { get; set; }
    string Content { get; set; }
}