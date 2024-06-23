using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GMCopilot.ApiCore.Services;

/// <summary>
/// Provides extraction of claims and access control for use by API controllers.
/// </summary>
public class AuthorizationService(ILogger<AuthorizationService> logger) : IAuthorizationService
{
    private ILogger<AuthorizationService> _logger => logger;

    private const string _oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string _nameClaimType = "name";

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
            _logger.LogError("No oid claim found in claims.");
            throw new InvalidOperationException("No oid claim.");
        }

        if (!Guid.TryParse(oidClaim.Value, out Guid oid))
        {
            _logger.LogError("Failed to parse oid claim {claimValue}.", oidClaim.Value);
            throw new InvalidOperationException("Failed to parse oid claim.");
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
        var nameClaim = context.User.Claims.FirstOrDefault(c => c.Type == _nameClaimType);
        
        if (null == nameClaim)
        {
            _logger.LogError("No name claim found in the user's claims.");
            throw new InvalidOperationException("No name claim.");
        }

        return nameClaim.Value;
    }

    /// <summary>
    /// Checks if the current request is made by an application.
    /// </summary>
    /// <param name="context">The HTTP context from the request</param>
    /// <returns>True if the request is made by an application, false otherwise.</returns>
    public bool IsAppMakingRequest(HttpContext context)
    {
        // Check for the presence of the "idtyp" claim to determine if the request is from an application
        var isAppTypeClaimPresent = context.User.Claims.Any(c => c.Type == "idtyp" && c.Value == "app");
        if (isAppTypeClaimPresent)
        {
            return true;
        }

        // If "idtyp" claim is not present, check for "roles" claim without "scp" claim as an indicator of an application request
        var hasRolesClaim = context.User.Claims.Any(c => c.Type == "roles");
        var lacksScpClaim = !context.User.Claims.Any(c => c.Type == "scp");

        return hasRolesClaim && lacksScpClaim;
    }

    /// <summary>
    /// Check if the current request is made by an application and has the specified permission.
    /// </summary>
    /// <param name="context">The HTTP context from the request</param>
    /// <param name="permission">The permission to check for.</param>
    /// <returns>True if the request is made by an application and has the specified permission, false otherwise.</returns>
    public bool AppHasPermission(HttpContext context, string permission)
    {
        return IsAppMakingRequest(context) && context.User.Claims.Any(c => c.Type == "roles" && c.Value == permission);
    }

    /// <summary>
    /// Checks if the current request can access the user with the specified userId.
    /// </summary>
    /// <param name="context">The HTTP context from the request</param>
    /// <param name="userId">The Id of the user to check access for.</param>
    /// <returns>True if the request can access the user, false otherwise.</returns>
    public bool RequestCanAccessUser(HttpContext context, Guid userId)
    {
        return IsAppMakingRequest(context) || (userId == GetUserId(context));
    }
}
