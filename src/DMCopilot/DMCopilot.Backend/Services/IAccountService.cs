using DMCopilot.Backend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DMCopilot.Backend.Services
{
    public interface IAccountService
    {
        public Task<Account> GetAccountAsync(AuthenticationState context);
    }
}
