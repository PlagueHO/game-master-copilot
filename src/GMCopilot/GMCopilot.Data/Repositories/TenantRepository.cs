using GMCopilot.Core.Models;

namespace GMCopilot.Data.Repositories;

public class TenantRepository : Repository<Tenant>
{
    /// <summary>
    /// Initializes a new instance of the TenantRepository class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public TenantRepository(IStorageContext<Tenant> storageContext)
        : base(storageContext)
    {
    }

    /// <summary>
    /// Finds the tenant using a tenant id.
    /// </summary>
    /// <param name="id">The tenant id.</param>
    /// <returns>The tenant record.</returns>
    public Task<Tenant> FindByTenantIdAsync(string tenantId)
    {
        return base.FindByIdAsync(tenantId);
    }

    /// <summary>
    /// Tries to find the tenant using a tenant id.
    /// </summary>
    /// <param name="id">The tenant id.</param>
    /// <returns>The tenant record.</returns>
    public async Task<bool> TryFindByTenantIdAsync(string tenantId, Action<Tenant?> tenant)
    {
        return await base.TryFindByIdAsync(tenantId, tenant);
    }

    /// <summary>
    /// Read all the tenants.
    /// </summary>
    /// <returns>A list of all tenant records.</returns>
    public Task<IEnumerable<Tenant>> ReadAllAsync()
    {
        return base.StorageContext.QueryEntitiesAsync(tenant => true);
    }   
}