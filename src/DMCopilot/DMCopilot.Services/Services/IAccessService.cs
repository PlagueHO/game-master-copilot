using DMCopilot.Entities.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DMCopilot.Services;

public interface IAccessService
{
    public Account Account { get; }
    public Tenant Tenant { get; }
    public bool IsLoaded { get; }

    public Task<Account> InitializeUsingContext(AuthenticationState context);
}