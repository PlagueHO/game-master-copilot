using DMCopilot.Entities.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DMCopilot.Data.Repositories;

public class WorldRepositoryCosmosDb : IWorldRepository
{
    private readonly Container _container;
    private readonly ILogger<WorldRepositoryCosmosDb> _logger;

    public WorldRepositoryCosmosDb(CosmosClient client, string databaseName, string containerName, ILogger<WorldRepositoryCosmosDb> logger)
    {
        _container = client.GetContainer(databaseName, containerName);
        _logger = logger;
        _logger.LogInformation($"Initialized {nameof(WorldRepositoryCosmosDb)} using container '{containerName}'.");
    }

    public async Task<World> GetWorldAsync(string id, string tenantId)
    {
        try
        {
            var response = await _container.ReadItemAsync<World>(id, GetPartitionKey(id, tenantId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new WorldNotFoundException($"World {id} not found in tenant {tenantId}");
        }
    }

    private async Task<World> GetWorldByQueryAsync(QueryDefinition queryDefinition)
    {
        var query = _container.GetItemQueryIterator<World>(queryDefinition);
        if (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            if (response != null && response.Count > 0)
            {
                return response.FirstOrDefault();
            }
        }
        return null;
    }

    public async Task<World> GetWorldByNameAsync(string tenantId, string name)
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.Name = @name")
            .WithParameter("@name", name);
        return await GetWorldByQueryAsync(queryDefinition);
    }

    private async Task<IEnumerable<World>> GetWorldsByQueryAsync(QueryDefinition queryDefinition)
    {
        var query = _container.GetItemQueryIterator<World>(queryDefinition);
        var results = new List<World>();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }
        return results;
    }

    public async Task<IEnumerable<World>> GetWorldsByTenantAsync(string tenantId)
    {
        var queryDefinition = new QueryDefinition("SELECT * FROM c");
        return await GetWorldsByQueryAsync(queryDefinition);
    }

    public async Task<World> CreateWorldAsync(World world)
    {
        var response = await _container.CreateItemAsync(world);
        return response.Resource;
    }

    public async Task<World> UpdateWorldAsync(string id, World world)
    {
        world.Id = id;
        var response = await _container.UpsertItemAsync(world, GetPartitionKey(world.Id, world.TenantId));
        return response.Resource;
    }

    public async Task<bool> DeleteWorldAsync(string id, string tenantId)
    {
        try
        {
            await _container.DeleteItemAsync<World>(id, GetPartitionKey(id, tenantId));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public static PartitionKey GetPartitionKey(string id, string tenantId)
    {
        return new PartitionKeyBuilder()
            .Add(tenantId)
            .Add(id)
            .Build();
    }
}