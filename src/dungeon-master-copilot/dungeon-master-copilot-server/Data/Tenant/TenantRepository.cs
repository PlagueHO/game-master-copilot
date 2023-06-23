using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dungeon_master_copilot_server.Data;
using Microsoft.Azure.Cosmos;

namespace dungeon_master_copilot_server.Data
{
    public class TenantRepository : ITenantRepository
    {
        private readonly Container _container;

        public TenantRepository(CosmosClient client, string databaseName, string containerName)
        {
            _container = client.GetContainer(databaseName, containerName);
        }

        public async Task<Tenant> GetTenantAsync(Guid id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Tenant>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new TenantNotFoundException(id.ToString());
            }
        }

        public async Task<Tenant> GetTenantByNameAsync(string name)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.Name = @name")
            .WithParameter("@name", name);

            var query = _container.GetItemQueryIterator<Tenant>(queryDefinition);
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

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            var query = _container.GetItemQueryIterator<Tenant>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<Tenant>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Tenant> CreateTenantAsync(Tenant tenant)
        {
            var response = await _container.CreateItemAsync(tenant);
            return response.Resource;
        }

        public async Task<Tenant> UpdateTenantAsync(Guid id, Tenant tenant)
        {
            tenant.Id = id;
            var response = await _container.UpsertItemAsync(tenant, new PartitionKey(tenant.Id.ToString()));
            return response.Resource;
        }

        public async Task<bool> DeleteTenantAsync(Guid id)
        {
            try
            {
                await _container.DeleteItemAsync<Tenant>(id.ToString(), new PartitionKey(id.ToString()));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}