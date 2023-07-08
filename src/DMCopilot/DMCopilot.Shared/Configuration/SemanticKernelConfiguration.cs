namespace DMCopilot.Shared.Configuration;

public class SemanticKernelConfiguration
{
    public string? PluginsDirectory { get; set; }
    public List<SemanticKernelAzureOpenAiTextCompletionServices>? AzureOpenAiTextCompletionServices { get; set; }
    public List<SemanticKernelAzureOpenAiChatCompletionServices>? AzureOpenAiChatCompletionServices { get; set; }
    public List<SemanticKernelAzureOpenAiTextEmbeddingGenerationServices>? AzureOpenAiTextEmbeddingGenerationServices { get; set; }
}

public class SemanticKernelAzureOpenAiTextCompletionServices
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool SetAsDefault { get; set; } = false;
}

public class SemanticKernelAzureOpenAiChatCompletionServices
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool AlsoAsTextCompletion { get; set; } = false;
    public bool SetAsDefault { get; set; } = false;
}

public class SemanticKernelAzureOpenAiTextEmbeddingGenerationServices
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool SetAsDefault { get; set; } = false;
}
