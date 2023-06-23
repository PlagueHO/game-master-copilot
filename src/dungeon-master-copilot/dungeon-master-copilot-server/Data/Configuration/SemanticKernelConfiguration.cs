using System.Collections.Generic;
using System.Text.Json;

namespace dungeon_master_copilot_server.Data.Configuration;

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
