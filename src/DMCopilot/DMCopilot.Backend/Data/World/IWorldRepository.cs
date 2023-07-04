using DMCopilot.Backend.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMCopilot.Backend.Data
{
    public interface IWorldRepository
    {
        Task<Tenant> GetWorldAsync(Guid worldId);
        Task<Tenant> GetWorldByNameAsync(string name);
        Task<Tenant> GetTenantByOwnerEmailAsync(EmailAddress ownerEmail);
        Task<IEnumerable<Tenant>> GetTenantsAsync();
        Task<IEnumerable<Tenant>> GetTenantsByOwnerEmailAsync(EmailAddress ownerEmail);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Tenant> UpdateTenantAsync(Guid worldId, Tenant tenant);
        Task<bool> DeleteTenantAsync(Guid worldId);
    }
}