using Microsoft.SemanticKernel;

namespace dungeon_master_copilot_server.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;

        public SemanticKernelService(IKernel semanticKernel)
        {
            _semanticKernel = semanticKernel;
        }
    }
}
