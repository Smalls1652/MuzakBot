namespace MuzakBot.App.Services;

public class CosmosDbServiceOptions
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;
}