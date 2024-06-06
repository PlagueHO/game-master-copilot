using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GMCopilot.Core.Services;

/// <summary>
/// Provides access to claims of current user.
/// </summary>
public class ClaimsProviderService
{
    private const string _oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private readonly ILogger<ClaimsProviderService> _logger;

    public ClaimsProviderService(ILogger<ClaimsProviderService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Extracts the User Id from the claims provided in an HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context from the request</param>
    /// <returns>A Guid containing the OID of the user.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Guid GetUserId(HttpContext context)
    {
        var oidClaim = context.User.Claims.FirstOrDefault(c => c.Type == _oidClaimType);
        if (null == oidClaim)
        {
            _logger.LogError("No oid claim found in the user's claims.");
            throw new InvalidOperationException("No oid claim!");
        }

        Guid oid;
        if (!Guid.TryParse(oidClaim.Value, out oid))
        {
            _logger.LogError($"Failed to parse oid claim value: {oidClaim.Value}");
            throw new InvalidOperationException("Failed to parse oid claim!");
        }

        return oid;
    }

    /// <summary>
    /// Extracts the User Name from the claims provided in an HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context from the request</param>
    /// <returns>A string containing the name of the user</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetUserName(HttpContext context)
    {
        var nameClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        if (null == nameClaim)
        {
            _logger.LogError("No name claim found in the user's claims.");
            throw new InvalidOperationException("No name claim!");
        }

        var name = nameClaim.Value;
        return name;
    }
}
