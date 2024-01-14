using Microsoft.Extensions.Logging;
using GMCopilot.Core.Models;

namespace GMCopilot.Services;

/// <summary>
/// Service for managing user accounts and tenants.
/// </summary>
public class AccessService : IAccessService
{
    /// <summary>
    /// Represents the authenticated user's account.
    /// </summary>
    private Account _account;
    public Account Account
    {
        get => _account;
        private set
        {
            _account = value;
            AccountChanged?.Invoke(this, value);
            _logger.LogInformation("Account changed to '{account}'.", value.Id);
        }
    }

    /// <summary>
    /// Event that is triggered when the account is changed.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="value">The new account value.</param>
    public event EventHandler<Account> AccountChanged = (sender, value) => { };

    /// <summary>
    /// Represents the authenticated user's active tenant.
    /// </summary>
    private Tenant _tenant;
    public Tenant Tenant
    {
        get => _tenant;
        private set
        {
            _tenant = value;
            TenantChanged?.Invoke(this, value);
            _logger.LogInformation("Tenant changed to '{tenant}'.", value.Id);
        }
    }

    /// <summary>
    /// Event that is triggered when the tenant is changed.
    /// </summary>
    /// <param name="sender">The object that triggered the event.</param>
    /// <param name="value">The new tenant value.</param>
    public event EventHandler<Tenant> TenantChanged = (sender, value) => { };

    /// <summary>
    /// Gets a value indicating whether the account and tenant are loaded.
    /// </summary>
    public bool IsLoaded => Account != null && Tenant != null;

    /// <summary>
    /// The logger for the AccessService class.
    /// </summary>
    private readonly ILogger<AccessService> _logger;

    public AccessService(ILogger<AccessService> logger)
    {
        _logger = logger;
    }
}