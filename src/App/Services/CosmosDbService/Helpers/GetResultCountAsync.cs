using Microsoft.Azure.Cosmos;

namespace MuzakBot.App.Services;

public partial class CosmosDbService
{
    /// <summary>
    /// Get the count of results for a specific query.
    /// </summary>
    /// <param name="container">The container to run the query against.</param>
    /// <param name="coreQuery">The core query to get the count of results.</param>
    /// <returns>The count of results that would be returned from the query.</returns>
    private async Task<int> GetResultCountAsync(Container container, string coreQuery)
    {
        // Create a query that gets the count of results.
        QueryDefinition countQuery = new($"SELECT VALUE COUNT(1) FROM c {coreQuery}");

        // Get the count of the results.
        FeedResponse<int> countQueryResponse = await container.GetItemQueryIterator<int>(
                queryDefinition: countQuery,
                requestOptions: new()
                {
                    MaxItemCount = 1
                }
            )
            .ReadNextAsync();

        // Get the count returned from the query.
        // There should only be one result, so the 'FirstOrDefault' method will be used to get the data.
        int count = countQueryResponse.FirstOrDefault();

        // Return the count.
        return count;
    }
}