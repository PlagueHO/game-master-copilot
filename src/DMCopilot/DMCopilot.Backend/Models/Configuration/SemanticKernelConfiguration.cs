namespace DMCopilot.Backend.Models.Configuration;

public enum SemanticKernelConfigurationServiceType
{
    AzureOpenAIServiceTextCompletion,
    AzureOpenAIServiceChatCompletion,
    AzureOpenAIServiceEmbedding
}
public class SemanticKernelConfiguration
{
    public string? PluginsDirectory { get; set; }
    public List<SemanticKernelConfigurationService>? Services { get; set; }
}

public class SemanticKernelConfigurationService
{
    public required string Id { get; set; }
    public SemanticKernelConfigurationServiceType Type { get; set; }
    public required string Endpoint { get; set; }
    public required string Deployment { get; set; }
}
