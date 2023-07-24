using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

public class AccountRepositoryNew : Repository<Account>
{
    /// <summary>
    /// Initializes a new instance of the AccountRepositoryNew class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public AccountRepositoryNew(IStorageContext<Account> storageContext)
        : base(storageContext)
    {
    }
}