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

    /// <summary>
    /// Finds a world using a world id.
    /// </summary>
    /// <param name="worldId">The world id</param>
    /// <returns>A world record with the world Id.</returns>
    public Task<World> FindByWorldIdAsync(string worldId, string tenantId)
    {
        return base.FindByIdAsync(worldId);
    }
    
    /// <summary>
    /// Finds the worlds using a tenant id.
    /// </summary>
    /// <param name="tenantId">The tenant id.</param>
    /// <returns>A list of world records with the tenant Id.</returns>
    public Task<IEnumerable<World>> FindByTenantIdAsync(string tenantId)
    {
        return base.StorageContext.QueryEntitiesAsync(e => e.TenantId == tenantId);
    }
}