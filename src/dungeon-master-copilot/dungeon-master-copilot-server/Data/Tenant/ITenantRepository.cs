using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dungeon_master_copilot_server.Data
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantAsync(Guid id);
        Task<Tenant> GetTenantByNameAsync(string name);
        Task<IEnumerable<Tenant>> GetTenantsAsync();
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Tenant> UpdateTenantAsync(Guid id, Tenant tenant);
        Task<bool> DeleteTenantAsync(Guid id);
    }
}