using GMCopilot.Core.Models;

namespace GMCopilot.Data.Repositories;

/// <summary>
/// Defines the basic CRUD operations for a repository.
/// This should not be used. Instead, should create an model specific interface that
/// extends this interface.
/// </summary>
public class Repository<T> : IRepository<T> where T : IStorageEntity
{
    /// <summary>
    /// The storage context.
    /// </summary>
    protected IStorageContext<T> StorageContext { get; set; }

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    public Repository(IStorageContext<T> storageContext)
    {
        StorageContext = storageContext;
    }

    /// <inheritdoc/>
    public Task CreateAsync(T entity)
    {
        return StorageContext.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(T entity)
    {
        return StorageContext.DeleteAsync(entity);
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(Guid id)
    {
        return StorageContext.ReadAsync(id);
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(Guid id, Guid tenantId)
    {
        return StorageContext.ReadAsync(id, tenantId);
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(Guid id, string type, Guid tenantId)
    {
        return StorageContext.ReadAsync(id, type, tenantId);
    }

    /// <inheritdoc/>
    public async Task<bool> TryFindByIdAsync(Guid id, Action<T?> entity)
    {
        try
        {
            entity(await FindByIdAsync(id));
            return true;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is KeyNotFoundException)
        {
            entity(default);
            return false;
        }
    }

    /// <inheritdoc/>
    public Task UpsertAsync(T entity)
    {
        return StorageContext.UpsertAsync(entity);
    }
}