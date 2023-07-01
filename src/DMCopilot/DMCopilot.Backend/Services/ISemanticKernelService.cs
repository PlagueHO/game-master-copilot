using Azure.Identity;
using DMCopilot.Backend.Models.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace DMCopilot.Backend.Services
{
    public interface ISemanticKernelService
    {
        public void LoadPlugin(string name);
        public Task<SKContext> InvokeFunctionAsync(string plugin, string function, string input);
        
    }
}


