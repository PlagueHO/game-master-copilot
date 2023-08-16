﻿using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

/// <summary>
/// Defines the extended CRUD operations for a tenanted repository.
/// </summary>
public interface IRepositoryTenanted<T> where T : IStorageTenantedEntity
{
    /// <summary>
    /// Finds an entity by its id and tenant id.
    /// </summary>
    /// <param name="id">Id of the entity.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>An entity</returns>
    Task<T> FindByIdAsync(string id, string tenantId);

    /// <summary>
    /// Tries to find an entity by its id and tenant id.
    /// </summary>
    /// <param name="id">Id of the entity.</param>
    /// <param name="tenantId">The tenant id.</param>
    /// <param name="entity">The entity delegate. Note async methods don't support ref or out parameters.</param>
    /// <returns>True if the entity was found, false otherwise.</returns>
    Task<bool> TryFindByIdAsync(string id, string tenantId, Action<T?> entity);
}