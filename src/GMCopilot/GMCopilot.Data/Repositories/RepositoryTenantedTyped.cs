using GMCopilot.Core.Models;

namespace GMCopilot.Data.Repositories;

/// <summary>
/// Defines the basic CRUD operations for a repository that is tenanted and typed.
/// </summary>
public class RepositoryTenantedTyped<T> : RepositoryTenanted<T>, IRepositoryTenantedTyped<T> where T : IStorageTenantedTypedEntity
{
    /// <summary>
    /// Initializes a new instance of the RepositoryTenantedType class.
    /// </summary>
    public RepositoryTenantedTyped(IStorageContext<T> storageContext)
        : base(storageContext)
    {
    }

    /// <inheritdoc/>
    public new Task<T> FindByIdAsync(string id, string type, string tenantId)
    {
        return StorageContext.ReadAsync(id, type, tenantId);
    }

    /// <inheritdoc/>
    public async Task<bool> TryFindByIdAsync(string id, string type, string tenantId, Action<T?> entity)
    {
        try
        {
            entity(await FindByIdAsync(id, type, tenantId));
            return true;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is KeyNotFoundException)
        {
            entity(default);
            return false;
        }
    }
}