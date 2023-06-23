using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace dungeon_master_copilot_server.Services
{
    public interface ISemanticKernelService
    {
        public Task<SKContext> InvokeFunctionAsync(string function, string input);
    }
}


