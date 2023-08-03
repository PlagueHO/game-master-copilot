using DMCopilot.Entities.Models;
using Microsoft.Azure.Cosmos;
using System.Net;

namespace DMCopilot.Data.Repositories;

/// <summary>
/// A storage context that stores entities in a CosmosDB container.
/// </summary>
public class CosmosDbContext<T> : IStorageContext<T>, IDisposable where T : IStorageEntity
{
    /// <summary>
    /// The CosmosDB client.
    /// </summary>
    private readonly CosmosClient _client;

    /// <summary>
    /// CosmosDB container.
    /// </summary>
    private readonly Container _container;

    /// <summary>
    /// Initializes a new instance of the CosmosDbContext class.
    /// </summary>
    /// <param name="connectionString">The CosmosDB connection string.</param>
    /// <param name="database">The CosmosDB database name.</param>
    /// <param name="container">The CosmosDB container name.</param>
    public CosmosDbContext(string connectionString, string database, string container)
    {
        // Configure JsonSerializerOptions
        var options = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            },
        };
        this._client = new CosmosClient(connectionString, options);
        this._container = this._client.GetContainer(database, container);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> QueryEntitiesAsync(Func<T, bool> predicate)
    {
        return await Task.Run<IEnumerable<T>>(
            () => this._container.GetItemLinqQueryable<T>(true).Where(predicate).AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task CreateAsync(T entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            throw new ArgumentOutOfRangeException(nameof(entity.Id), "Entity Id cannot be null or empty.");
        }

        await this._container.CreateItemAsync(entity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(T entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            throw new ArgumentOutOfRangeException(nameof(entity.Id), "Entity Id cannot be null or empty.");
        }

        await this._container.DeleteItemAsync<T>(entity.Id, BuildPartitionKey(entity));
    }

    /// <summary>
    /// Read an entity from the storage context by id and partition key.
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="partitionKey"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    internal async Task<T> ReadAsync(string entityId, PartitionKey partitionKey)
    {
        try
        {
            var response = await this._container.ReadItemAsync<T>(entityId, partitionKey);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException($"Entity with id {entityId} not found.");
        }
    }

    /// <inheritdoc/>
    public async Task<T> ReadAsync(string entityId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
        {
            throw new ArgumentOutOfRangeException(nameof(entityId), "Entity Id cannot be null or empty.");
        }

        return await ReadAsync(entityId, BuildPartitionKey(entityId));
    }

    /// <inheritdoc/>
    public async Task<T> ReadAsync(string entityId, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
        {
            throw new ArgumentOutOfRangeException(nameof(entityId), "Entity Id cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentOutOfRangeException(nameof(tenantId), "Tenant Id cannot be null or empty.");
        }

        return await ReadAsync(entityId, BuildPartitionKey(entityId, tenantId));
    }

    /// <inheritdoc/>
    public async Task<T> ReadAsync(string entityId, string type, string tenantId)
    {
        if (string.IsNullOrWhiteSpace(entityId))
        {
            throw new ArgumentOutOfRangeException(nameof(entityId), "Entity Id cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), "Type cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentOutOfRangeException(nameof(tenantId), "Tenant Id cannot be null or empty.");
        }

        return await ReadAsync(entityId, BuildPartitionKey(entityId, type, tenantId));
    }

    /// <inheritdoc/>
    public async Task UpsertAsync(T entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            throw new ArgumentOutOfRangeException(nameof(entity.Id), "Entity Id cannot be null or empty.");
        }

        await this._container.UpsertItemAsync(entity);
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._client.Dispose();
        }
    }

    /// <summary>
    /// Build the partition key for the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The partition key.</returns>
    internal PartitionKey BuildPartitionKey(string entityId)
    {
        return new PartitionKeyBuilder().Add(entityId).Build();
    }

    /// <summary>
    /// Build the partition key for the entity.
    /// </summary>
    /// <param name="entityid">The entity id.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>The partition key.</returns>
    internal PartitionKey BuildPartitionKey(string entityId, string tenantId)
    {
        return new PartitionKeyBuilder().Add(entityId).Add(tenantId).Build();
    }

    /// <summary>
    /// Build the partition key for the entity.
    /// </summary>
    /// <param name="entityid">The entity id.</param>
    /// <param name="type">The entity type.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>The partition key.</returns>
    internal PartitionKey BuildPartitionKey(string entityId, string type, string tenantId)
    {
        return new PartitionKeyBuilder().Add(entityId).Add(type).Add(tenantId).Build();
    }

    /// <summary>
    /// Build the partition key for the entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The partition key.</returns>
    internal PartitionKey BuildPartitionKey(T entity)
    {
        var partitionKeyBuilder = new PartitionKeyBuilder().Add(entity.Id);

        if (entity is ITenantStorageEntity tenantEntity)
            partitionKeyBuilder.Add(tenantEntity.TenantId);

        if (entity is ITypedTenantStorageEntity tenantStorageEntity)
            partitionKeyBuilder.Add(tenantStorageEntity.Type);

        return partitionKeyBuilder.Build();
    }
}