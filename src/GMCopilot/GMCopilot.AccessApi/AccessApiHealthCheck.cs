using Microsoft.Extensions.Diagnostics.HealthChecks;

public class AccessApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        // TODO: Check something to determine if the API is healthy

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("Access API is healthy."));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "Access API is healthy."));
    }
}