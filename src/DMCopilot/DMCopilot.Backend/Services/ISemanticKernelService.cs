using Azure.Identity;
using DMCopilot.Backend.Data.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace DMCopilot.Backend.Services
{
    public interface ISemanticKernelService
    {
        public Task<SKContext> InvokeFunctionAsync(string function, string input);
    }
}


