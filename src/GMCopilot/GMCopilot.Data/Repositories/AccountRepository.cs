using GMCopilot.Core.Models;

namespace GMCopilot.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    /// <summary>
    /// The storage context.
    /// </summary>
    protected IStorageContext<Account> StorageContext { get; set; }

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// </summary>
    public AccountRepository(IStorageContext<Account> storageContext)
    {
        StorageContext = storageContext;
    }

    /// <inheritdoc/>
    public Task CreateAsync(Account account)
    {
        return StorageContext.CreateAsync(account);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Account account)
    {
        return StorageContext.DeleteAsync(account);
    }

    /// <inheritdoc/>
    public Task<Account> FindByIdAsync(Guid id)
    {
        return StorageContext.ReadAsync(id);
    }

    /// <inheritdoc/>
    public async Task<bool> TryFindByIdAsync(Guid id, Action<Account?> account)
    {
        try
        {
            account(await FindByIdAsync(id));
            return true;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is KeyNotFoundException)
        {
            account(default(Account?));
            return false;
        }
    }

    /// <inheritdoc/>
    public Task UpsertAsync(Account account)
    {
        return StorageContext.UpsertAsync(account);
    }
}