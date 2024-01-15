using MuzakBot.App.Models.CommandModules;

namespace MuzakBot.App.Models.Database.LyricsAnalyzer;

public interface ILyricsAnalyzerPromptStyle
{
    string Id { get; set; }
    string PartitionKey { get; set; }
    string Name { get; set; }
    string ShortName { get; set; }
    string AnalysisType { get; set; }
    string Prompt { get; set; }
    string UserPrompt { get; set; }
    string NoticeText { get; set; }
    DateTimeOffset CreatedOn { get; set; }
    DateTimeOffset LastUpdated { get; set; }

    void UpdateFromModalInput(LyricsAnalyzerPromptStyleModal inputModal);
    void UpdateLastUpdateTime();
}
