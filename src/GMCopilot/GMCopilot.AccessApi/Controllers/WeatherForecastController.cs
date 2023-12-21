using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GMCopilot.Core.Authorization;

namespace GMCopilot.AccessApi.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;

    public WeatherForecastController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationScopes.UserRead)]
    public async Task<IActionResult> Get()
    {
        try
        {
            var weatherSummaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast(
                    DateTime.Now.AddDays(index),
                    Random.Shared.Next(-20, 55),
                    weatherSummaries[Random.Shared.Next(weatherSummaries.Length)]
                ))
                .ToArray();
            return Ok(forecast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting forecast.");
            return StatusCode(500);
        }
    }
}

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}