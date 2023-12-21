﻿namespace GMCopilot.AccessApi;

/// <summary>
/// Provides access to claims of current user.
/// </summary>
public class ClaimsProviderService
{
    private const string _oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public Guid GetUserId(HttpContext context)
    {
        var oidClaim = context.User.Claims.FirstOrDefault(c => c.Type == _oidClaimType);
        if (null == oidClaim)
            throw new InvalidOperationException("No oid claim!");

        var oid = Guid.Parse(oidClaim.Value);
        return oid;
    }
}
