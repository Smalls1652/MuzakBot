using MuzakBot.Database.Models;

namespace MuzakBot.App;

[JsonSerializable(typeof(DatabaseConfigOptions))]
[JsonSerializable(typeof(SqliteConfigOptions))]
[JsonSerializable(typeof(CosmosDbConfigOptions))]
internal partial class AppConfigJsonContext : JsonSerializerContext
{
}
