using GMCopilot.Core.Models;

namespace GMCopilot.Data.Repositories;

public class TenantRepository : ITenantRepository
{
    /// <summary>
    /// The storage context.
    /// </summary>
    protected IStorageContext<Tenant> StorageContext { get; set; }

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    public TenantRepository(IStorageContext<Tenant> storageContext)
    {
        StorageContext = storageContext;
    }

    /// <inheritdoc/>
    public Task CreateAsync(Tenant tenant)
    {
        return StorageContext.CreateAsync(tenant);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Tenant tenant)
    {
        return StorageContext.DeleteAsync(tenant);
    }

    /// <inheritdoc/>
    public Task<Tenant> FindByIdAsync(Guid id)
    {
        return StorageContext.ReadAsync(id);
    }

    /// <inheritdoc/>
    public Task<Tenant> FindByIdAsync(Guid id, Guid tenantId)
    {
        return StorageContext.ReadAsync(id, tenantId);
    }

    /// <inheritdoc/>
    public async Task<bool> TryFindByIdAsync(Guid id, Action<Tenant?> tenant)
    {
        try
        {
            tenant(await FindByIdAsync(id));
            return true;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is KeyNotFoundException)
        {
            tenant(default(Tenant?));
            return false;
        }
    }

    /// <inheritdoc/>
    public Task UpsertAsync(Tenant tenant)
    {
        return StorageContext.UpsertAsync(tenant);
    }
}
