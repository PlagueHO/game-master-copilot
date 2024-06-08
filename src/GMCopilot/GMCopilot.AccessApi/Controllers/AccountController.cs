using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using GMCopilot.Core.Authorization;
using GMCopilot.Core.Services;

namespace GMCopilot.AccessApi.Controllers;

/// <summary>
/// API for managing accounts.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly AccountRepository _accountRepository;
    private readonly TenantRepository _tenantRepository;
    private readonly ClaimsProviderService _claimsProvider;

    public AccountController(
        ILogger<AccountController> logger,
        AccountRepository accountRepository,
        TenantRepository tenantRepository,
        ClaimsProviderService claimsProvider)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _tenantRepository = tenantRepository;
        _claimsProvider = claimsProvider;
    }

    // Create a test API that allows anonymous access
    [HttpGet("Test", Name = "Test")]
    [AllowAnonymous]
    public ActionResult Test()
    {
        return Ok("Test");
    }

    /// <summary>
    /// Get a user account for the current user.
    /// It will create a new account and tenant if one does not exist.
    /// </summary>
    /// <returns>The account record of the user.</returns>
    [HttpGet("GetOrCreateAccount", Name = "GetOrCreateAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotRead)]
    public async Task<ActionResult<Account>> GetOrCreateAccount()
    {
        // TODO: Refactor this method and move it into an AccessService
        try
        {
            var accountId = _claimsProvider.GetUserId(HttpContext);
            var account = await _accountRepository.FindByAccountIdAsync(accountId);
            
            if (account == null)
            {
                var userName = _claimsProvider.GetUserName(HttpContext);               

                var tenantRoles = new List<AccountTenantRole> {
                    new(accountId, TenantType.Individual, TenantRole.Owner)
                };

                // Create a new account
                account = new Account(accountId, userName, tenantRoles);
                await _accountRepository.CreateAsync(account);

                // Create a new individual tenant for the account
                var tenant = new Tenant(accountId, userName, accountId, TenantType.Individual);
                await _tenantRepository.CreateAsync(tenant);
            }

            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the account of the currently logged in user.
    /// </summary>
    /// <returns>The account record of the user.</returns>
    [HttpGet(Name = "GetAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotRead)]
    public async Task<ActionResult<Account>> GetAccount()
    {
        try
        {
            var account = await _accountRepository.FindByAccountIdAsync(_claimsProvider.GetUserId(HttpContext));
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Updates the account of the currently logged in user.
    /// </summary>
    /// <param name="account">The account to update.</param>
    /// <returns>An HTTP result code.</returns>
    [HttpPut(Name = "UpdateAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotReadWrite)]
    public async Task<ActionResult> UpdateAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != _claimsProvider.GetUserId(HttpContext))
            {
                // Can't change the account of another user without AccountsAdmin scope
                return Unauthorized();
            }

            await _accountRepository.UpsertAsync(account);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Creates a new account for the current user.
    /// </summary>
    /// <param name="account">The account to create.</param>
    /// <returns>An HTTP result code.</returns>
    [HttpPost(Name = "CreateAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotReadWrite)]
    public async Task<ActionResult> CreateAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != _claimsProvider.GetUserId(HttpContext))
            {
                // Can't create an account for another user without AccountsAdmin scope
                return Unauthorized();
            }

            await _accountRepository.CreateAsync(account);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the account of a specific user.
    /// </summary>
    /// <param name="id">The Id of the user to get the account for.</param>
    /// <returns>The account record of the user.</returns>
    [HttpGet("{id}", Name = "GetAccountById")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotRead)]
    public async Task<ActionResult<Account>> GetAccountById(Guid id)
    {
        try
        {
            var account = await _accountRepository.FindByAccountIdAsync(id);
            return (account == null ? NotFound() : Ok(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns>An HTTP result code.</returns>
    [HttpDelete("{id}", Name = "DeleteAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotReadWrite)]
    public async Task<ActionResult> DeleteAccount(Guid id)
    {
        try
        {
            var account = await _accountRepository.FindByAccountIdAsync(id);
            await _accountRepository.DeleteAsync(account);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account.");
            return StatusCode(500);
        }
    }
}
