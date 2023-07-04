using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMCopilot.Backend.Models;

namespace DMCopilot.Backend.Data
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountAsync(Guid id);
        Task<Account> GetAccountByEmailAsync(string email);
        Task<IEnumerable<Account>> GetAccountsAsync();
        Task<Account> CreateAccountAsync(Account account);
        Task<Account> UpdateAccountAsync(string email, Account account);
        Task<bool> DeleteAccountAsync(string email);
    }
}
