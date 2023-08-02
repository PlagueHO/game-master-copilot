using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

public class AccountRepository : Repository<Account>
{
    /// <summary>
    /// Initializes a new instance of the AccountRepositoryNew class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public AccountRepository(IStorageContext<Account> storageContext)
        : base(storageContext)
    {
    }

    /// <summary>
    /// Founds the account record using account id.
    /// </summary>
    /// <param name="id">The account id.</param>
    /// <returns>The account record.</returns>
    public Task<Account> FindByAccountIdAsync(string accountId)
    {
        return base.StorageContext.ReadAsync(accountId);
    }
}