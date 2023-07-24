using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

public interface IAccountRepository
{
    Task<Account> GetAccountAsync(string id);
    Task<IEnumerable<Account>> GetAccountsAsync();
    Task<Account> CreateAccountAsync(Account account);
    Task<Account> UpdateAccountAsync(string id, Account account);
    Task<bool> DeleteAccountAsync(string id);
}
