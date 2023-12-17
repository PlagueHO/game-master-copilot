using GMCopilot.Core.Models;

namespace GMCopilot.Core.Repositories;

/// <summary>
/// Defines the basic CRUD operations for a repository.
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
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            throw new ArgumentOutOfRangeException(nameof(entity.Id), "Entity Id cannot be null or empty.");
        }

        return StorageContext.CreateAsync(entity);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(T entity)
    {
        return StorageContext.DeleteAsync(entity);
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(string id)
    {
        return StorageContext.ReadAsync(id);
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(string id, string tenantId)
    {
        return StorageContext.ReadAsync(id, tenantId);
    }

    /// <inheritdoc/>
    public Task<T> FindByIdAsync(string id, string type, string tenantId)
    {
        return StorageContext.ReadAsync(id, type, tenantId);
    }

    /// <inheritdoc/>
    public async Task<bool> TryFindByIdAsync(string id, Action<T?> entity)
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