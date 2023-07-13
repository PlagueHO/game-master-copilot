namespace DMCopilot.Shared.Configuration;

public class SemanticKernelConfiguration
{
    public string? AzureOpenAiApiKey { get; set; }
    public string? PluginsDirectory { get; set; }
    public List<SemanticKernelAzureOpenAiTextCompletionService>? AzureOpenAiTextCompletionServices { get; set; }
    public List<SemanticKernelAzureOpenAiChatCompletionService>? AzureOpenAiChatCompletionServices { get; set; }
    public List<SemanticKernelAzureOpenAiTextEmbeddingGenerationService>? AzureOpenAiTextEmbeddingGenerationServices { get; set; }
    public List<SemanticKernelAzureOpenAiImageGenerationService>? AzureOpenAiImageGenerationServices { get; set; }
}

public class SemanticKernelAzureOpenAiTextCompletionService
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool SetAsDefault { get; set; } = false;
}

public class SemanticKernelAzureOpenAiChatCompletionService
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool AlsoAsTextCompletion { get; set; } = false;
    public bool SetAsDefault { get; set; } = false;
}

public class SemanticKernelAzureOpenAiTextEmbeddingGenerationService
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool SetAsDefault { get; set; } = false;
}

public class SemanticKernelAzureOpenAiImageGenerationService
{
    public string? Id { get; set; }
    public string? Deployment { get; set; }
    public string? Endpoint { get; set; }
    public bool SetAsDefault { get; set; } = false;
}
