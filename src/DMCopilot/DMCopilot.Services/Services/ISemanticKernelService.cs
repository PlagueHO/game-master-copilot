using Microsoft.SemanticKernel.Orchestration;

namespace DMCopilot.Services;

public interface ISemanticKernelService
{
    public Task<SKContext> InvokePluginFunctionAsync(string plugin, string function, Dictionary<string, string> inputs);
}