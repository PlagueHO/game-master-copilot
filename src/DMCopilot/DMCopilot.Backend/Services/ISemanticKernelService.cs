using Azure.Identity;
using DMCopilot.Backend.Models.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace DMCopilot.Backend.Services
{
    public interface ISemanticKernelService
    {
        public Task<SKContext> InvokeFunctionAsync(string function, string input);
    }
}


