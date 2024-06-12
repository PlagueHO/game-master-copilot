using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using GMCopilot.Core.Services;
using Microsoft.Identity.Web.Resource;

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
    private readonly AuthorizationService _authorizationService;

    public AccountController(
        ILogger<AccountController> logger,
        AccountRepository accountRepository,
        TenantRepository tenantRepository,
        AuthorizationService authorizationService)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _tenantRepository = tenantRepository;
        _authorizationService = authorizationService;

        // Log that the AccountController has been created
        _logger.LogInformation("AccountController created.");
    }

    /// <summary>
    /// Initializes an account for the logged in user.
    /// It will create a new account and tenant if one does not exist.
    /// It will also create a new individual tenant for the account if one does not exist.
    /// It can only be called when user claims are provided.
    /// </summary>
    /// <returns>The account record of the user.</returns>
    [HttpGet("Initialize", Name = "InitializeAccount")]
    [RequiredScope(["GMCopilot.Read"])]
    public async Task<ActionResult<Account>> InitializeAccount()
    {
        try
        {
            var accountId = _authorizationService.GetUserId(HttpContext);
            var account = await _accountRepository.FindByAccountIdAsync(accountId);

            if (account == null)
            {
                var userName = _authorizationService.GetUserName(HttpContext);

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
            _logger.LogError(ex, "Error initializing account.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the account of the currently logged in user.
    /// </summary>
    /// <returns>The account record of the user.</returns>
    [HttpGet(Name = "GetAccount")]
    [RequiredScope(["GMCopilot.Read"])]
    public async Task<ActionResult<Account>> GetAccount()
    {
        try
        {
            var account = await _accountRepository.FindByAccountIdAsync(_authorizationService.GetUserId(HttpContext));
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Creates a new account for the currently logged in user.
    /// </summary>
    /// <param name="account">The account to create.</param>
    /// <returns>An HTTP result code.</returns>
    [HttpPost(Name = "CreateAccount")]
    [RequiredScopeOrAppPermission(["GMCopilot.ReadWrite.All"], ["GMCopilot.ReadWrite"])]
    public async Task<ActionResult> CreateAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != _authorizationService.GetUserId(HttpContext))
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
    /// Updates the account of the currently logged in user.
    /// </summary>
    /// <param name="account">The account to update.</param>
    /// <returns>An HTTP result code.</returns>
    [HttpPut(Name = "UpdateAccount")]
    [RequiredScope(["GMCopilot.ReadWrite"])]
    public async Task<ActionResult> UpdateAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != _authorizationService.GetUserId(HttpContext))
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
    /// Deletes the account of the currently logged in user.
    /// </summary>
    /// <returns>An HTTP result code.</returns>
    [HttpDelete(Name = "DeleteAccount")]
    [RequiredScope(["GMCopilot.ReadWrite"])]
    public async Task<ActionResult> DeleteAccount()
    {
        try
        {
            var account = await _accountRepository.FindByAccountIdAsync(_authorizationService.GetUserId(HttpContext));
            await _accountRepository.DeleteAsync(account);
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
    [RequiredScopeOrAppPermission(["GMCopilot.Read.All"], ["GMCopilot.Read"])]
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
    /// Update the account of a specific user.
    /// </summary>
    /// <param name="id">The Id of the user to update the account for.</param>
    /// <param name="account">The account to update.</param>
    /// <returns>An HTTP result code.</returns>
    [HttpPut("{id}", Name = "UpdateAccountById")]
    [RequiredScopeOrAppPermission(["GMCopilot.ReadWrite.All"], ["GMCopilot.ReadWrite"])]
    public async Task<ActionResult> UpdateAccount(Guid id, Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != id)
            {
                // Can't change the account of another user without 
                return Unauthorized();
            }

            await _accountRepository.UpsertAsync(account);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Deletes the account of a specific user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>An HTTP result code.</returns>
    [HttpDelete("{id}", Name = "DeleteAccountById")]
    [RequiredScopeOrAppPermission(["GMCopilot.ReadWrite.All"], ["GMCopilot.ReadWrite"])]
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
