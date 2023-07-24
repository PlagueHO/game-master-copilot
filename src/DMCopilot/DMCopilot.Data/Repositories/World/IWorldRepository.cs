using DMCopilot.Data.Models;

namespace DMCopilot.Data.Repositories;

public interface IWorldRepository
{
    Task<World> GetWorldAsync(string id, string tenantId);
    Task<World> GetWorldByNameAsync(string tenantId, string name);
    Task<IEnumerable<World>> GetWorldsByTenantAsync(string tenantId);
    Task<World> CreateWorldAsync(World world);
    Task<World> UpdateWorldAsync(string id, World world);
    Task<bool> DeleteWorldAsync(string id, string tenantId);
}