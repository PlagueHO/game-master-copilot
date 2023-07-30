using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

public class WorldRepository : Repository<World>
{
    /// <summary>
    /// Initializes a new instance of the WorldRepository class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public WorldRepository(IStorageContext<World> storageContext)
        : base(storageContext)
    {
    }

    public Task<IEnumerable<World>> FindByTenantAsync(string tenantId)
    {
        return base.StorageContext.QueryEntitiesAsync(e => e.TenantId == tenantId);
    }
}