using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using DMCopilot.Backend.Data;
using DMCopilot.Backend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DMCopilot.Backend.Services
{
    /// <summary>
    /// Service for managing user accounts and authentication.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository, ITenantRepository tenantRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _tenantRepository = tenantRepository;
            _logger = logger;
            _logger.LogInformation("Creating Account Service");
        }

        /// <summary>
        /// Retrieves the account details for the specified user or creates a new account if one does not exist.
        /// </summary>
        /// <param name="context">The authentication state for the user.</param>
        /// <returns>The account details for the user.</returns>
        public async Task<Account> GetAccountAsync(AuthenticationState context)
        {
            Account account;
            if (context.User.Identity?.Name == null)
            {
                throw new NotAuthenticatedException();
            }

            try
            {
                account = await _accountRepository.GetAccountAsync(context.User.Identity.Name);
            }
            catch (AccountNotFoundException)
            {
                account = new Account(
                    context.User.Identity.Name,
                    context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                );
                await _accountRepository.CreateAccountAsync(account);
            }
            return account;
        }
    }
}