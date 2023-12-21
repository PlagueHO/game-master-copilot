using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Microsoft.Graph;
using GMCopilot.Core.Models;
using GMCopilot.Core.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace GMCopilot.AccessApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class AccountController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly AccountRepository _accountRepository;

    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger, GraphServiceClient graphServiceClient, AccountRepository accountRepository)
    {
        _logger = logger;
        _graphServiceClient = graphServiceClient;
        _accountRepository = accountRepository;
    }

    
    // GET: Account of currently logged in user
    [HttpGet(Name = "GetAccountCurrentUser")]
    public async Task<ActionResult<Account>> Get()
    {
        try
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();
            var account = await _accountRepository.FindByAccountIdAsync(user.Id);
            return Ok(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by id.");
            return StatusCode(500);
        }
    }

    // GET: Account by Id
    [HttpGet("{id}", Name = "GetAccountById")]
    public async Task<ActionResult<Account>> Get(string id)
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

    // POST: Account
    [HttpPost(Name = "CreateAccount")]
    public async Task<ActionResult> Create(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
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

    // PUT: Account
    [HttpPut("{id}", Name = "UpsertAccount")]
    public async Task<ActionResult> Upsert(Account account)
    {
        try
        {
            if (account == null)
            {
                return BadRequest();
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

    // DELETE: Account by Id
    [HttpDelete("{id}", Name = "DeleteAccount")]
    public async Task<ActionResult> Delete(string id)
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
