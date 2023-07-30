using DMCopilot.Entities.Models;
using Microsoft.Graph.Models;

namespace DMCopilot.Data.Repositories;

public interface ITenantRepository
{
    Task<Tenant> GetTenantAsync(string id);

    Task<Tenant> GetTenantByNameAsync(string name);

    Task<Tenant> GetTenantByOwnerEmailAsync(EmailAddress ownerEmail);

    Task<IEnumerable<Tenant>> GetTenantsAsync();

    Task<IEnumerable<Tenant>> GetTenantsByOwnerEmailAsync(EmailAddress ownerEmail);

    Task<Tenant> CreateTenantAsync(Tenant tenant);

    Task<Tenant> UpdateTenantAsync(string id, Tenant tenant);

    Task<bool> DeleteTenantAsync(string id);
}