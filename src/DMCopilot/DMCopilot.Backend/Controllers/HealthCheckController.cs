using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMCopilot.Backend.Controllers;

[ApiController]
[Route("healthcheck")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;

    public HealthCheckController(ILogger<HealthCheckController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        bool healthy = true;
        // Perform health checks

        // Return the health check status
        if (healthy)
        {
            _logger.LogInformation("Healthcheck OK");
            return Ok();
        }
        else
        {
            _logger.LogWarning("Healthcheck failed");
            return StatusCode(500);
        }
    }
}