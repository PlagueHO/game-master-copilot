using DMCopilot.Entities.Models;
using Microsoft.Graph.Models;

namespace DMCopilot.Data.Repositories;

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
    /// Gets the tenant using a tenant id
    /// </summary>
    /// <param name="id">The tenant id.</param>
    /// <returns>The tenant record.</returns>
    public Task<Tenant> GetByTenantIdAsync(string id)
    {
        return base.StorageContext.ReadAsync(id);
    }
}