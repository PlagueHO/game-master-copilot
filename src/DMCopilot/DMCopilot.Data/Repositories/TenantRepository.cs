using DMCopilot.Entities.Models;
using Microsoft.Graph.Models;

namespace DMCopilot.Data.Repositories;

public class TenantRepositoryNew : Repository<Tenant>
{
    /// <summary>
    /// Initializes a new instance of the TenantRepository class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public TenantRepositoryNew(IStorageContext<Tenant> storageContext)
        : base(storageContext)
    {
    }
}