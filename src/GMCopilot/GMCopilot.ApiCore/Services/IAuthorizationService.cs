using Microsoft.AspNetCore.Http;

namespace GMCopilot.ApiCore.Services
{
    public interface IAuthorizationService
    {
        public Guid GetUserId(HttpContext context);
        public string GetUserName(HttpContext context);
        public bool IsAppMakingRequest(HttpContext context);
        public bool AppHasPermission(HttpContext context, string permission);
        public bool RequestCanAccessUser(HttpContext context, Guid userId);
    }
}
