using DMCopilot.Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DMCopilot.Backend.Controllers
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
