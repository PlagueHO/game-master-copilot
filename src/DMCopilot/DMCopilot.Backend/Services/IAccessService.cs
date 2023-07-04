using DMCopilot.Backend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DMCopilot.Backend.Services
{
    public interface IAccessService
    {
        public Task<Account> LoadAccountAsync(AuthenticationState context);
    }
}
