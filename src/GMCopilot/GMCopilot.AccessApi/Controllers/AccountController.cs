using Microsoft.AspNetCore.Mvc;
using GMCopilot.Core.Models;
using GMCopilot.Data.Repositories;
using GMCopilot.ApiCore.Services;
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
    private readonly IAccountRepository _accountRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IAuthorizationService _authorizationService;

    public AccountController(
        ILogger<AccountController> logger,
        IAccountRepository accountRepository,
        ITenantRepository tenantRepository,
        IAuthorizationService authorizationService)
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
    /// It will create a new account if one does not exist.
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
            if (_authorizationService.IsAppMakingRequest(HttpContext))
            {
                return Unauthorized("Application request not permitted.");
            }

            // Get the user ID from the claims
            var userIdFromClaims = _authorizationService.GetUserId(HttpContext);

            // Does the user account exist?
            Account? account = null;
            await _accountRepository.TryFindByIdAsync(userIdFromClaims, (Account? a) => account = a);

            if (account == null)
            {
                _logger.LogInformation($"Account with ID {userIdFromClaims} not found.");

                // The user account does not exist, so create it
                var userNameFromClaims = _authorizationService.GetUserName(HttpContext);

                var tenantRoles = new List<AccountTenantRole> {
                    new(userIdFromClaims, TenantType.Individual, TenantRole.Owner)
                };

                // Create a new account
                account = new Account(userIdFromClaims, userNameFromClaims, tenantRoles);
                await _accountRepository.CreateAsync(account);

                _logger.LogInformation($"Account created for user ID {userIdFromClaims}.");

                // Does the individual tenant for the account exist?
                Tenant? tenant = null;
                await _tenantRepository.TryFindByIdAsync(userIdFromClaims, (Tenant? t) => tenant = t);

                if (tenant == null)
                {
                    // Create a new individual tenant for the account
                    tenant = new Tenant(userIdFromClaims, userNameFromClaims, userIdFromClaims, TenantType.Individual);
                    await _tenantRepository.CreateAsync(tenant);

                    _logger.LogInformation($"Tenant (Individual) created for user ID {userIdFromClaims}.");
                }
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
    /// Create an account for a user.
    /// Can be called by an application or the user themselves.
    /// If called by an application then any user's account can be created
    /// as long as they have the GMCopilot.ReadWrite.All permission.
    /// If called by a user then they can only create an account for themselves.
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

            if (!_authorizationService.RequestCanAccessUser(HttpContext, account.Id))
            {
                return Unauthorized("Creating account unauthorized by user.");
            }

            await _accountRepository.CreateAsync(account);

            return CreatedAtRoute("GetAccountById", new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Gets the account of a specific user.
    /// Can be called by an application or the user themselves.
    /// If called by an application then any user's account can be accessed
    /// as long as they have the GMCopilot.Read.All permission.
    /// If called by a user then they can only access their own account.
    /// </summary>
    /// <param name="id">The Id of the user to get the account for.</param>
    /// <returns>The account record of the user.</returns>
    [HttpGet("{id}", Name = "GetAccountById")]
    [RequiredScopeOrAppPermission(["GMCopilot.Read.All"], ["GMCopilot.Read"])]
    public async Task<ActionResult<Account>> GetAccountById(Guid id)
    {
        try
        {
            if (!_authorizationService.RequestCanAccessUser(HttpContext, id))
            {
                return Unauthorized("Getting account unauthorized by user.");
            }

            Account? account = null;
            await _accountRepository.TryFindByIdAsync(id, (Account? a) => account = a);
            return (account == null ? NotFound() : Ok(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Update the account of a specific user by Id.
    /// Can be called by an application or the user themselves.
    /// If called by an application then any user's account can be updated
    /// as long as they have the GMCopilot.ReadWrite.All permission.
    /// If called by a user then they can only access their own account.
    /// If the account already exists, it will be updated.
    /// </summary>
    /// <param name="id">The Id of the user to update the account for.</param>
    /// <param name="account">The account to update.</param>
    /// <returns>An HTTP result code.</returns>
    [HttpPut("{id}", Name = "UpdateAccountById")]
    [RequiredScopeOrAppPermission(["GMCopilot.ReadWrite.All"], ["GMCopilot.ReadWrite"])]
    public async Task<ActionResult> UpdateAccountById(Guid id, Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (!_authorizationService.RequestCanAccessUser(HttpContext, id))
            {
                return Unauthorized("Updating account unauthorized by user.");
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
    /// Deletes the account of a specific user by Id.
    /// Can be called by an application or the user themselves.
    /// If called by an application then any user's account can be deleted
    /// as long as they have the GMCopilot.ReadWrite.All permission.
    /// If called by a user then they can only access their own account.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>An HTTP result code.</returns>
    [HttpDelete("{id}", Name = "DeleteAccountById")]
    [RequiredScopeOrAppPermission(["GMCopilot.ReadWrite.All"], ["GMCopilot.ReadWrite"])]
    public async Task<ActionResult> DeleteAccountById(Guid id)
    {
        try
        {
            if (!_authorizationService.RequestCanAccessUser(HttpContext, id))
            {
                return Unauthorized("Deleting account unauthorized by user.");
            }

            Account? account = null;
            await _accountRepository.TryFindByIdAsync(id, (Account? a) => account = a);

            if (account == null)
            {
                return NotFound();
            }

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
