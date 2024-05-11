namespace MuzakBot.Lib.Services.Logging.CosmosDb;

/// <summary>
/// Holds constants for logging to Cosmos DB.
/// </summary>
public static class CosmosDbServiceLoggingConstants
{
    /// <summary>
    /// Holds the names of operations for logging to Cosmos DB.
    /// </summary>
    public static class OperationTypes
    {
        /// <summary>
        /// An item was created/added to the database.
        /// </summary>
        public const string Added = "added";

        /// <summary>
        /// An item was updated in the database.
        /// </summary>
        public const string Updated = "updated";

        /// <summary>
        /// An item was retrieved from the database.
        /// </summary>
        public const string Found = "found";
    }

    /// <summary>
    /// Holds the names of item types for logging to Cosmos DB.
    /// </summary>
    public static class ItemTypes
    {
        /// <summary>
        /// The lyrics analyzer user rate limit item type.
        /// </summary>
        public const string LyricsAnalyzerUserRateLimit = "lyrics analyzer user rate limit";

        /// <summary>
        /// The lyrics analyzer song lyrics item type.
        /// </summary>
        public const string LyricsAnalyzerSongLyrics = "song lyrics item";

        /// <summary>
        /// The lyrics analyzer config item type.
        /// </summary>
        public const string LyricsAnalyzerConfig = "lyrics analyzer config";

        /// <summary>
        /// The lyrics analyzer prompt style item type.
        /// </summary>
        public const string LyricsAnalyzerPromptStyle = "lyrics analyzer prompt style";
    }
}
