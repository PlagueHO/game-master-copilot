using dungeon_master_copilot_server.Services;
using Microsoft.AspNetCore.Mvc;

namespace dungeon_master_copilot_server.Controllers
{
    public class SemanticKernelController : Controller
    {
        private readonly ISemanticKernelService _semanticKernelService;

        public SemanticKernelController(ISemanticKernelService semanticKernelService)
        {
            _semanticKernelService = semanticKernelService;
        }
    }
}
