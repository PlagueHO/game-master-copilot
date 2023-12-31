using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace GMCopilot.Core.Services;

/// <summary>
/// Provides access to claims of current user.
/// </summary>
public class ClaimsProviderService
{
    private const string _oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

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
            throw new InvalidOperationException("No oid claim!");

        var oid = Guid.Parse(oidClaim.Value);
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
            throw new InvalidOperationException("No name claim!");

        var name = nameClaim.Value;
        return name;
    }
}
