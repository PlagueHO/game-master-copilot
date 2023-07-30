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
    /// Gets the account using account id which is usually the e-mail address of the user
    /// </summary>
    /// <param name="id">The account id.</param>
    /// <returns>The account record.</returns>
    public Task<Account> GetByAccountIdAsync(string id)
    {
        return base.StorageContext.ReadAsync(id);
    }
}