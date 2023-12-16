using GMCopilot.Core.Models;

namespace GMCopilot.Data.Repositories;

/// <summary>
/// Defines the basic CRUD operations for a storage context.
/// </summary>
public interface IStorageContext<T> where T : IStorageEntity
{
    /// <summary>
    /// Query entities in the storage context.
    /// </summary>
    Task<IEnumerable<T>> QueryEntitiesAsync(Func<T, bool> predicate);

    /// <summary>
    /// Read an entity from the storage context by id.
    /// </summary>
    /// <param name="entityId">The entity id.</param>
    /// <returns>The entity.</returns>
    Task<T> ReadAsync(string entityId);

    /// <summary>
    /// Read an entity from a multi-tenanted storage context by id and tenant id.
    /// </summary>
    /// <param name="entityId">The entity id.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>The entity.</returns>
    Task<T> ReadAsync(string entityId, string tenantId);

    /// <summary>
    /// Read an entity from a multi-typed, multi-tenanted storage context by id and tenant id.
    /// </summary>
    /// <param name="entityId">The entity id.</param>
    /// <param name="type">The type of the entity.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>The entity.</returns>
    Task<T> ReadAsync(string entityId, string type, string tenantId);

    /// <summary>
    /// Create an entity in the storage context.
    /// </summary>
    /// <param name="entity">The entity to be created in the context.</param>
    Task CreateAsync(T entity);

    /// <summary>
    /// Upsert an entity in the storage context.
    /// </summary>
    /// <param name="entity">The entity to be upserted in the context.</param>
    Task UpsertAsync(T entity);

    /// <summary>
    /// Delete an entity from the storage context.
    /// </summary>
    /// <param name="entity">The entity to be deleted from the context.</param>
    Task DeleteAsync(T entity);
}