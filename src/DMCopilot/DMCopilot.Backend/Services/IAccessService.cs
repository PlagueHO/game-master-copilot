using DMCopilot.Backend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DMCopilot.Backend.Services
{
    public interface IAccessService
    {
        public Account Account { get; }
        public Tenant Tenant { get; }
        public Boolean IsLoaded { get; }
        public Task<Account> InitializeUsingContext(AuthenticationState context);
    }
}
