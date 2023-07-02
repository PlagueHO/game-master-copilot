using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Linq;
using System.Threading.Tasks;

namespace DMCopilot.Backend.Controllers
{
    [ApiController]
    [Route("healthcheck")]
    public class HealthCheckController : ControllerBase
    {
        private readonly CosmosClient _cosmosClient;

        public HealthCheckController(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            bool healthy = true;

            // Check the Cosmos DB connection
            var databases = await _cosmosClient.GetDatabaseQueryIterator<DatabaseProperties>().ReadNextAsync();
            healthy = databases.Any() && healthy;

            // Perform other health checks here

            // Return the health check status
            if (healthy)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500);
            }
        }
    }

}
