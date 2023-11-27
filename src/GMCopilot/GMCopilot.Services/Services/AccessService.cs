using GMCopilot.Data.Repositories;
using GMCopilot.Entities.Models;
using GMCopilot.Services.Exceptions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace GMCopilot.Services;

/// <summary>
/// Service for managing user accounts and tenants.
/// </summary>
public class AccessService : IAccessService
{
    public Account? Account { get; private set; }
    public Tenant? Tenant { get; private set; }
    public bool IsLoaded => Account != null && Tenant != null;
    private readonly AccountRepository _accountRepository;
    private readonly TenantRepository _tenantRepository;
    private readonly ILogger<AccessService> _logger;

    public AccessService(ILogger<AccessService> logger, AccountRepository accountRepository, TenantRepository tenantRepository)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _tenantRepository = tenantRepository;
    }

    /// <summary>
    /// Uses the authentication context to get or create the account and tenant for the user.
    /// </summary>
    /// <param name="context">The authentication state for the user.</param>
    /// <returns>The account details for the user.</returns>
    public async Task<Account> InitializeUsingContext(AuthenticationState context)
    {
        // If the authentication context for the user is null, then fail
        if (context.User == null || context.User.Identity?.Name == null)
        {
            throw new AccessServiceNotAuthenticatedException();
        }

        // Obtain the email address of the user from the authentication context
        var email = context.User.Identity.Name;

        try
        {
            // See if the account already exists
            Account = await _accountRepository.FindByAccountIdAsync(email);

            try
            {
                // The accounts exists, so check if the tenant matching the active tenant exists
                Tenant = await _tenantRepository.FindByTenantIdAsync(Account.ActiveTenantId);
            }
            catch (KeyNotFoundException)
            {
                // The tenant does not exist, so create it
                Tenant = await InitializeTenantAsync(context);
            }
        }
        catch (KeyNotFoundException)
        {
            // The account does not exist so create it
            Account = await InitializeAccountAsync(context);
        }

        return Account;
    }

    /// <summary>
    /// Initializes a new account using the authentication context.
    /// </summary>
    /// <param name="context">The authentication state for the user.</param>
    /// <returns>The newly initialized account.</returns>
    private async Task<Account> InitializeAccountAsync(AuthenticationState context)
    {
        // If the authentication context for the user is null, then fail
        if (context.User.Identity?.Name == null)
        {
            throw new AccessServiceNotAuthenticatedException();
        }

        // Create a new individual tenant for the account
        var tenant = await InitializeTenantAsync(context);

        if (tenant == null)
        {
            throw new AccessServiceTenantCreateException("Failed to create tenant.");
        }

        // Create the account for the user and associate the individual tenant with it
        var email = context.User.Identity.Name;
        var name = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;

        var tenantRoles = new List<AccountTenantRole> {
            new AccountTenantRole(tenant.Id, email, name, TenantType.Individual, TenantRole.Owner)
        };
        var account = new Account(email, name, tenant.Id, tenantRoles);
        await _accountRepository.CreateAsync(account);

        _logger.LogInformation("Creating account for '{email}'.", email);

        return account;
    }

    /// <summary>
    /// Creates a new individual tenant for the currently logged in user.
    /// </summary>
    /// <param name="context">The currently authenticated user.</param>
    /// <returns>The newly initialized tenant.</returns>
    /// <exception cref="AccessServiceNotAuthenticatedException"></exception>
    private async Task<Tenant> InitializeTenantAsync(AuthenticationState context)
    {
        // If the authentication context for the user is null, then fail
        if (context.User.Identity?.Name == null)
        {
            throw new AccessServiceNotAuthenticatedException();
        }

        // Create a new individual tenant for the user account
        var email = context.User.Identity.Name;
        var name = context.User.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? email;
        var tenant = new Tenant(Guid.NewGuid().ToString(), name, email, TenantType.Individual);
        await _tenantRepository.CreateAsync(tenant);

        _logger.LogInformation("Creating individual tenant for '{email}'.", email);

        return tenant;
    }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>A string that represents the object.</returns>
    public override string ToString()
    {
        if (IsLoaded)
        {
            return $"{Account?.Name} ({Tenant?.Name})";
        }
        else
        {
            return string.Empty;
        }
    }

}