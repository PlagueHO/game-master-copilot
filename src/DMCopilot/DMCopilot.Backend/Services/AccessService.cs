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
    public class AccessService : IAccessService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<AccessService> _logger;

        public AccessService(IAccountRepository accountRepository, ITenantRepository tenantRepository, ILogger<AccessService> logger)
        {
            _accountRepository = accountRepository;
            _tenantRepository = tenantRepository;
            _logger = logger;
            _logger.LogInformation("Creating Access Service");
        }

        /// <summary>
        /// Loads the account details for the specified user or creates a new account if one does not exist.
        /// </summary>
        /// <param name="context">The authentication state for the user.</param>
        /// <returns>The account details for the user.</returns>
        public async Task<Account> LoadAccountAsync(AuthenticationState context)
        {
            Account account;

            if (context.User.Identity?.Name == null)
            {
                throw new NotAuthenticatedException();
            }

            var email = context.User.Identity.Name;

            try
            {
                account = await _accountRepository.GetAccountByEmailAsync(email);
            }
            catch (AccountNotFoundException)
            {
                account = await InitializeAccountAsync(context);
            }

            return account;
        }

        private async Task<Account> InitializeAccountAsync(AuthenticationState context)
        {
            if (context.User.Identity?.Name == null)
            {
                throw new NotAuthenticatedException();
            }

            // Create a new individual tenant for the account
            var tenant = await InitializeTenantAsync(context);

            // Create the account for the user and associate the individual tenant with it
            var email = context.User.Identity.Name;
            var name = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;

            if (tenant == null)
            {
                throw new TenantNotFoundException("Tenant could not be created.");
            }

            var tenantRoles = new List<AccountTenantRole> {
                new AccountTenantRole(tenant.Id, name, TenantRole.Owner)
            };

            var account = new Account(Guid.NewGuid(), email, name, tenant.Id, tenantRoles);
            await _accountRepository.CreateAccountAsync(account);
            
            _logger.LogInformation("Creating account for '{email}'.", email);
            
            return account;
        }

        private async Task<Tenant> InitializeTenantAsync(AuthenticationState context)
        {
            if (context.User.Identity?.Name == null)
            {
                throw new NotAuthenticatedException();
            }

            // Create a new individual tenant for the user account
            var email = context.User.Identity.Name;
            var name = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;
            var tenant = new Tenant(Guid.NewGuid(), name, email, TenantType.Individual);
            await _tenantRepository.CreateTenantAsync(tenant);
            
            _logger.LogInformation("Creating individual tenant for '{email}'.", email);
            
            return tenant;
        }
    }
}