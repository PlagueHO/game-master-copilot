using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMCopilot.Backend.Models;

namespace DMCopilot.Backend.Data
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountAsync(String id);
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(String id, Account account);
        Task<bool> DeleteAccountAsync(String ir);
    }
}
