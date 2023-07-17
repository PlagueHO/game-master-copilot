using DMCopilot.Shared.Data;
using DMCopilot.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DMCopilot.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorldController : ControllerBase
    {
        private IWorldRepository _worldRepository;
        private ILogger<WorldController> _logger;

        public WorldController(IWorldRepository worldRepository, ILogger<WorldController> logger)
        {
            _worldRepository = worldRepository;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IEnumerable<World>> Get([FromQuery] Guid tenantId)
        {
            return await _worldRepository.GetWorldsByTenantAsync(tenantId);
        }

        [HttpGet("{id}")]
        public async Task<World> Get(Guid id, [FromQuery] Guid tenantId)
        {
            return await _worldRepository.GetWorldAsync(id, tenantId);
        }

        [HttpPost]
        public async Task Post([FromBody] World world)
        {
            await _worldRepository.CreateWorldAsync(world);
        }

        [HttpPut("{id}")]
        public async Task Put(Guid id, [FromBody] World world)
        {
            await _worldRepository.UpdateWorldAsync(id, world);
        }

        // DELETE api/<WorldController>/5
        [HttpDelete("{id}")]
        public async Task Delete(Guid id, [FromBody] Guid tenantId)
        {
            await _worldRepository.DeleteWorldAsync(id, tenantId);
        }
    }
}
