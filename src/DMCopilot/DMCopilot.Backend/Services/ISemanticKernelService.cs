using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace DMCopilot.Services
{
    public interface ISemanticKernelService
    {
        public Task<SKContext> InvokeFunctionAsync(string function, string input);
    }
}


