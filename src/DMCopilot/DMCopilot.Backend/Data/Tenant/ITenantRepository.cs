using DMCopilot.Backend.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMCopilot.Backend.Data
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantAsync(Guid id);
        Task<Tenant> GetTenantByNameAsync(String name);
        Task<Tenant> GetTenantByOwnerEmailAsync(EmailAddress ownerEmail);
        Task<IEnumerable<Tenant>> GetTenantsAsync();
        Task<IEnumerable<Tenant>> GetTenantsByOwnerEmailAsync(EmailAddress ownerEmail);
        Task<Tenant> CreateTenantAsync(Tenant tenant);
        Task<Tenant> UpdateTenantAsync(Guid id, Tenant tenant);
        Task<bool> DeleteTenantAsync(Guid id);
    }
}