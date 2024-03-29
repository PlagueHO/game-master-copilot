﻿using GMCopilot.Entities.Models;

namespace GMCopilot.Data.Repositories;

public class UniverseRepository : RepositoryTenanted<Universe>
{
    /// <summary>
    /// Initializes a new instance of the UniverseRepository class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public UniverseRepository(IStorageContext<Universe> storageContext)
        : base(storageContext)
    {
    }

    /// <summary>
    /// Finds a universe using a universe id.
    /// </summary>
    /// <param name="universeid">The universe id</param>
    /// <returns>A universe record with the universe Id.</returns>
    public Task<Universe> FindByUniverseIdAsync(string universeId, string tenantId)
    {
        return base.FindByIdAsync(universeId, tenantId);
    }
    
    /// <summary>
    /// Finds the universes using a tenant id.
    /// </summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>A list of universe records with the tenant Id.</returns>
    public Task<IEnumerable<Universe>> FindByTenantIdAsync(string tenantId)
    {
        return base.StorageContext.QueryEntitiesAsync(e => e.TenantId == tenantId);
    }
}