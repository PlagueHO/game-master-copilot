using DMCopilot.Data.Models;

namespace DMCopilot.Data.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountAsync(String id);
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(String id, Account account);
        Task<Boolean> DeleteAccountAsync(String ir);
    }
}
