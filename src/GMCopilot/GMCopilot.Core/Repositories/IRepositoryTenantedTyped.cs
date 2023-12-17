using GMCopilot.Core.Models;

namespace GMCopilot.Core.Repositories;

/// <summary>
/// Defines the extended CRUD operations for a tenanted and typed repository.
/// </summary>
public interface IRepositoryTenantedTyped<T> where T : IStorageTenantedTypedEntity
{
    /// <summary>
    /// Finds an entity by its id, tenant id and type.
    /// </summary>
    /// <param name="id">Id of the entity.</param>
    /// <param name="type">The type of the entity.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>An entity</returns>
    Task<T> FindByIdAsync(string id, string type, string tenantId);

    /// <summary>
    /// Tries to find an entity by its id, tenant id and type.
    /// </summary>
    /// <param name="id">Id of the entity.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="type">The type of the entity.</param>
    /// <param name="entity">The entity delegate. Note async methods don't support ref or out parameters.</param>
    /// <returns>True if the entity was found, false otherwise.</returns>
    Task<bool> TryFindByIdAsync(string id, string type, string tenantId, Action<T?> entity);
}