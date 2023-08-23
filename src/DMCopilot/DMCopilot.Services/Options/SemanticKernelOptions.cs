using System.ComponentModel.DataAnnotations;

namespace DMCopilot.Services.Options;

/// <summary>
/// Semantic Kernel options for DMCopilot.
/// </summary>
public class SemanticKernelOptions
{
    public const string PropertyName = "SemanticKernel";

    /// <summary>
    /// Supported types of AI services.
    /// </summary>
    public enum AIServiceType
    {
        /// <summary>
        /// Azure OpenAI https://learn.microsoft.com/en-us/azure/cognitive-services/openai/
        /// </summary>
        AzureOpenAI,

        /// <summary>
        /// OpenAI https://openai.com/
        /// </summary>
        OpenAI
    }

    public class AzureOpenAiTextCompletionService
    {
        /// <summary>
        /// The Semantic Kernel identifier for the Azure OpenAI Text Completion model.
        /// </summary>
        public string Id { get; set; } = "TextCompletion";

        /// <summary>
        /// The endpoint to use to call the model in Azure OpenAI Service.
        /// </summary>
        [Url, Required]
        public string? Endpoint { get; set; }

        /// <summary>
        /// The deployment name of the model in Azure OpenAI Service.
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? Deployment { get; set; }

        /// <summary>
        /// Should this model be the default model for this type.
        /// </summary>
        public bool SetAsDefault { get; set; } = false;
    }

    public class AzureOpenAiChatCompletionService
    {
        /// <summary>
        /// The Semantic Kernel identifier for the Azure OpenAI Chat Completion model.
        /// </summary>
        public string Id { get; set; } = "ChatCompletion";

        /// <summary>
        /// The endpoint to use to call the model in Azure OpenAI Service.
        /// </summary>
        [Url, Required]
        public string? Endpoint { get; set; }

        /// <summary>
        /// The deployment name of the model in Azure OpenAI Service.
        /// </summary>
        [NotEmptyOrWhitespace]
        public string? Deployment { get; set; }

        /// <summary>
        /// Should this model be the default model for this type.
        /// </summary>
        public bool SetAsDefault { get; set; } = false;

        /// <summary>
        /// Whether to also use this model for Text Completions.
        /// </summary>
        public bool AlsoAsTextCompletion { get; set; } = true;

        /// <summary>
        /// Key to access the AI service.
        /// </summary>
        [NotEmptyOrWhitespace]
        public string ApiKey { get; set; } = string.Empty;
    }

    public class AzureOpenAiTextEmbeddingGenerationService
    {
        /// <summary>
        /// The Semantic Kernel identifier for the Azure OpenAI Embedding model.
        /// </summary>
        public string Id { get; set; } = "Embeddings";

        /// <summary>
        /// The endpoint to use to call the model in Azure OpenAI Service.
        /// </summary>
        [Url, Required]
        public string? Endpoint { get; set; }

        /// <summary>
        /// The deployment name of the model in Azure OpenAI Service.
        /// </summary>
        [Required, NotEmptyOrWhitespace]
        public string? Deployment { get; set; }

        /// <summary>
        /// Should this model be the default model for this type.
        /// </summary>
        public bool SetAsDefault { get; set; } = false;

        /// <summary>
        /// Key to access the AI service.
        /// </summary>
        [NotEmptyOrWhitespace]
        public string ApiKey { get; set; } = string.Empty;
    }

    public class AzureOpenAiImageService
    {
        /// <summary>
        /// The Semantic Kernel identifier for the Azure OpenAI Image model.
        /// </summary>
        public string Id { get; set; } = "ImageGeneration";

        /// <summary>
        /// The endpoint to call the Image Generation model.
        /// </summary>
        [Url, Required]
        public string? Endpoint { get; set; }

        /// <summary>
        /// Should this model be the default model for this type.
        /// </summary>
        public bool SetAsDefault { get; set; } = false;

        /// <summary>
        /// Key to access the AI service.
        /// </summary>
        [NotEmptyOrWhitespace]
        public string ApiKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// The Semantic Kernel Plugins directory
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string? PluginsDirectory { get; set; }

    /// <summary>
    /// Type of AI service to use for Semantic Kernel.
    /// </summary>
    [Required]
    public AIServiceType Type { get; set; } = AIServiceType.AzureOpenAI;

    /// <summary>
    /// When <see cref="Type"/> is <see cref="AuthorizationType.AzureOpenAI"/> this is the Azure OpenAI Text Completion service to use if Type is AzureOpenAI.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), AIServiceType.AzureOpenAI, notEmptyOrWhitespace: true)]
    public List<AzureOpenAiTextCompletionService> AzureOpenAiTextCompletionServices { get; set; } = new List<AzureOpenAiTextCompletionService>();

    /// <summary>
    /// When <see cref="Type"/> is <see cref="AuthorizationType.AzureOpenAI"/> this is the Azure OpenAI Chat Completion service to use if Type is AzureOpenAI.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), AIServiceType.AzureOpenAI, notEmptyOrWhitespace: true)]
    public List<AzureOpenAiChatCompletionService> AzureOpenAiChatCompletionServices { get; set; } = new List<AzureOpenAiChatCompletionService>();

    /// <summary>
    /// When <see cref="Type"/> is <see cref="AuthorizationType.AzureOpenAI"/> this is the Azure OpenAI Embedding service to use if Type is AzureOpenAI.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), AIServiceType.AzureOpenAI, notEmptyOrWhitespace: true)]
    public List<AzureOpenAiTextEmbeddingGenerationService> AzureOpenAiTextEmbeddingGenerationServices { get; set; } = new List<AzureOpenAiTextEmbeddingGenerationService>();

    /// <summary>
    /// When <see cref="Type"/> is <see cref="AuthorizationType.AzureOpenAI"/> this is the Azure OpenAI Image service to use if Type is AzureOpenAI.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), AIServiceType.AzureOpenAI, notEmptyOrWhitespace: true)]
    public List<AzureOpenAiImageService> AzureOpenAiImageServices { get; set; } = new List<AzureOpenAiImageService>();

    /// <summary>
    /// Key to access the Azure Open AI service.
    /// </summary>
    [Required, NotEmptyOrWhitespace]
    public string AzureOpenAiApiKey { get; set; } = string.Empty;
}