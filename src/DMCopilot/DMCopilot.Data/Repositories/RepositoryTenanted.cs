using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

/// <summary>
/// Defines the basic CRUD operations for a repository that is tenanted.
/// </summary>
public class RepositoryTenanted<T> : Repository<T>, IRepositoryTenanted<T> where T : IStorageEntity
{
    /// <summary>
    /// Initializes a new instance of the RepositoryTenanted class.
    /// </summary>
    public RepositoryTenanted(IStorageContext<T> storageContext)
        : base(storageContext)
    {
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(string id, string tenantId)
    {
        return StorageContext.ReadAsync(id, tenantId);
    }

    /// <inheritdoc/>
    public async Task<bool> TryFindByIdAsync(string id, string tenantId, Action<T?> entity)
    {
        try
        {
            entity(await FindByIdAsync(id, tenantId));
            return true;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is KeyNotFoundException)
        {
            entity(default);
            return false;
        }
    }
}