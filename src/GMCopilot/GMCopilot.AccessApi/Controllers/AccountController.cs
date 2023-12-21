using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GMCopilot.Core.Models;
using GMCopilot.Core.Repositories;
using GMCopilot.Core.Authorization;

namespace GMCopilot.AccessApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly AccountRepository _accountRepository;
    private readonly ClaimsProviderService _claimsProvider;

    public AccountController(
        ILogger<AccountController> logger,
        AccountRepository accountRepository,
        ClaimsProviderService claimsProvider)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _claimsProvider = claimsProvider;
    }

    /// <summary>
    /// Gets the user ID from the claims.
    /// </summary>
    /// <returns>The user ID as a string.</returns>
    private string GetUserIdFromClaims()
    {
        // TODO: Change all StorageEntityId to be Guids
        return _claimsProvider.GetUserId(HttpContext).ToString();
    }
    
    // <summary>
    // Gets the account of the currently logged in user.
    // </summary>
    [HttpGet(Name = "GetAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotUser)]
    public async Task<ActionResult<Account>> GetAccount()
    {
        try
        {
            var account = await _accountRepository.FindByAccountIdAsync(GetUserIdFromClaims());
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    // <summary>
    // Updates the account of the currently logged in user.
    // </summary>
    // <param name="account">The account to update.</param>
    [HttpPut(Name = "UpdateAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotUser)]
    public async Task<ActionResult> UpdateAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != GetUserIdFromClaims())
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

    // <summary>
    // Creates a new account for the current user.
    // </summary>
    // <param name="account">The account to create.</param>
    [HttpPost(Name = "CreateAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotUser)]
    public async Task<ActionResult> CreateAccount(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Id != GetUserIdFromClaims())
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

    // <summary>
    // Gets the account of a specific user.
    // </summary>
    // <param name="id">The ID of the user to get the account for.</param>
    [HttpGet("{id}", Name = "GetAccountById")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotAdmin)]
    public async Task<ActionResult<Account>> GetAccountById(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var account = await _accountRepository.FindByAccountIdAsync(id);
            return (account == null ? NotFound() : Ok(account));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    // DELETE: Account by Id
    [HttpDelete("{id}", Name = "DeleteAccount")]
    [Authorize(Policy = AuthorizationScopes.GMCopilotAdmin)]
    public async Task<ActionResult> DeleteAccount(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

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
