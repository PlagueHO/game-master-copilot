using GMCopilot.Core.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GMCopilot.AccessApi.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        private const string _scopeClaimType = "http://schemas.microsoft.com/identity/claims/scope";

        public static IServiceCollection AddAccessAuthorization(this IHostApplicationBuilder builder)
        {
            return builder.Services.AddAuthorization(options =>
            {
                AddPolicy(options, AuthorizationScopes.UserRead);
                AddPolicy(options, AuthorizationScopes.GMCopilotRead);
                AddPolicy(options, AuthorizationScopes.GMCopilotReadWrite);
            });
        }

        private static void AddPolicy(AuthorizationOptions options, string scope)
        {
            options.AddPolicy(scope, policy =>
            {
                policy.RequireAssertion(context =>
                {
                    return context.User.HasClaim(claim =>
                    {
                        return claim.Type == _scopeClaimType && claim.Value.Contains(scope);
                    });
                });
            });
        }
    }
}
