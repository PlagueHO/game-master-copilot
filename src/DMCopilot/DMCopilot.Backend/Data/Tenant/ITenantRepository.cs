using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMCopilot.Backend.Data
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantAsync(Guid tenantId);
        Task<Tenant> GetTenantByNameAsync(string name);
        Task<IEnumerable<Tenant>> GetTenantsAsync();
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Tenant> UpdateTenantAsync(Guid tenantId, Tenant tenant);
        Task<bool> DeleteTenantAsync(Guid tenantId);
    }
}