using Azure.Identity;
using GMCopilot.Services.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

namespace GMCopilot.Services;

public class SemanticKernelService : ISemanticKernelService
{
    private readonly IKernel _semanticKernel;
    private readonly ILogger<SemanticKernelService> _logger;
    private readonly SemanticKernelOptions _options;
    private readonly DefaultAzureCredential? _azureCredential;
    private readonly string _pluginsDirectory;
    private IDictionary<string, IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction>> _functions;

    public SemanticKernelService(ILogger<SemanticKernelService> logger, IOptions<SemanticKernelOptions> options)
    {
        _logger = logger;
        _options = options.Value;

        // TODO: Move the kernel creation into execution of a plan
        _logger.LogInformation("Creating Semantic Kernel");

        var semanticKernelBuilder = new KernelBuilder();

        // Load the generative AI services that will be available to Semantic Kernel
        LoadAzureOpenAiTextCompletionServices(semanticKernelBuilder);
        LoadAzureOpenAiChatCompletionServices(semanticKernelBuilder);
        LoadAzureOpenAiTextEmbeddingGenerationServices(semanticKernelBuilder);
        LoadAzureOpenAiImageGenerationServices(semanticKernelBuilder);

        _semanticKernel = semanticKernelBuilder.Build();

        // Set the class-level private field _skillDirectory to a value that
        // corresponds to the value retrieved from the configuration or the default
        // value specified in the code block, which is "\Plugins".
        var pluginsDirectory = _options.PluginsDirectory;
        if (pluginsDirectory == null)
        {
            _pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        }
        else
        {
            _pluginsDirectory = Path.IsPathRooted(pluginsDirectory) ? pluginsDirectory : Path.Combine(Directory.GetCurrentDirectory(), pluginsDirectory);
        }

        _functions = new Dictionary<string, IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction>>();
    }

    public SemanticKernelService(ILogger<SemanticKernelService> logger, DefaultAzureCredential azureCredential, IOptions<SemanticKernelOptions> options) : this(logger, options)
    {
    }

    public void LoadAzureOpenAiTextCompletionServices(KernelBuilder semanticKernelBuilder)
    {
        if (_options.AzureOpenAiTextCompletionServices == null)
            return;

        foreach (var service in _options.AzureOpenAiTextCompletionServices)
        {
            _logger.LogInformation($"Adding Azure OpenAI Text Completion service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

            if (_options.AzureOpenAiApiKey == null)
                semanticKernelBuilder.WithAzureTextCompletionService(
                   deploymentName: service.Deployment,
                   endpoint: service.Endpoint,
                   credentials: _azureCredential,
                   serviceId: service.Id,
                   setAsDefault: service.SetAsDefault);
            else
                semanticKernelBuilder.WithAzureTextCompletionService(
                   deploymentName: service.Deployment,
                   endpoint: service.Endpoint,
                   apiKey: _options.AzureOpenAiApiKey,
                   serviceId: service.Id,
                   setAsDefault: service.SetAsDefault);
        }
    }

    public void LoadAzureOpenAiChatCompletionServices(KernelBuilder semanticKernelBuilder)
    {
        if (_options.AzureOpenAiChatCompletionServices == null)
            return;

        foreach (var service in _options.AzureOpenAiChatCompletionServices)
        {
            _logger.LogInformation($"Adding Azure OpenAI Chat Completion service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

            if (_options.AzureOpenAiApiKey == null)
                semanticKernelBuilder.WithAzureChatCompletionService(
                    deploymentName: service.Deployment,
                    endpoint: service.Endpoint,
                    credentials: _azureCredential,
                    serviceId: service.Id,
                    setAsDefault: service.SetAsDefault,
                    alsoAsTextCompletion: service.AlsoAsTextCompletion);
            else
                semanticKernelBuilder.WithAzureChatCompletionService(
                    deploymentName: service.Deployment,
                    endpoint: service.Endpoint,
                    apiKey: _options.AzureOpenAiApiKey,
                    serviceId: service.Id,
                    setAsDefault: service.SetAsDefault,
                    alsoAsTextCompletion: service.AlsoAsTextCompletion);
        }
    }

    public void LoadAzureOpenAiTextEmbeddingGenerationServices(KernelBuilder semanticKernelBuilder)
    {
        if (_options.AzureOpenAiTextEmbeddingGenerationServices == null)
            return;

        foreach (var service in _options.AzureOpenAiTextEmbeddingGenerationServices)
        {
            _logger.LogInformation($"Adding Azure OpenAI Text Embedding Generation service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

            if (_options.AzureOpenAiApiKey == null)
                semanticKernelBuilder.WithAzureTextEmbeddingGenerationService(
                    deploymentName: service.Deployment,
                    endpoint: service.Endpoint,
                    credential: _azureCredential,
                    serviceId: service.Id,
                    setAsDefault: service.SetAsDefault);
            else
                semanticKernelBuilder.WithAzureTextEmbeddingGenerationService(
                    deploymentName: service.Deployment,
                    endpoint: service.Endpoint,
                    apiKey: _options.AzureOpenAiApiKey,
                    serviceId: service.Id,
                    setAsDefault: service.SetAsDefault);
        }
    }

    public void LoadAzureOpenAiImageGenerationServices(KernelBuilder semanticKernelBuilder)
    {
        if (_options.AzureOpenAiImageServices == null)
            return;

        foreach (var service in _options.AzureOpenAiImageServices)
        {
            _logger.LogInformation($"Adding Azure OpenAI Image Generation service '{service.Id}' on endpoint '{service.Endpoint}' to Semantic Kernel");

            if (_options.AzureOpenAiApiKey == null)
                _logger.LogWarning("AzureOpenAiApiKey can not be empty if Azure OpenAI Image Generation Service is being used. Image Generation service will not be available.");

            semanticKernelBuilder.WithAzureOpenAIImageGenerationService(
               endpoint: service.Endpoint,
               apiKey: _options.AzureOpenAiApiKey,
               serviceId: service.Id,
               setAsDefault: service.SetAsDefault);
        }
    }

    public void LoadPlugin(string name)
    {
        if (!_functions.ContainsKey(name))
        {
            // Import a Semantic Skill from the specified directory and output a console message to confirm it.
            _functions.Add(name, _semanticKernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, name));
            _logger.LogInformation($"Imported {_functions.Count()} from plugin '{name} in the '{_pluginsDirectory}' directory.");
        }
    }

    public async Task<SKContext> InvokePluginFunctionAsync(string plugin, string function, Dictionary<string, string> inputs)
    {
        LoadPlugin(plugin);
        var context = _semanticKernel.CreateNewContext();
        foreach (string input in inputs.Keys)
            context[input] = inputs[input];
        _logger.LogInformation($"Invoking '{function}' from plugin '{plugin}.");
        var result = await _functions[plugin][function].InvokeAsync(context);

        // TODO: Improve error handling
        if (result.ErrorOccurred)
            throw new SemanticKernelSkillException($"Error occurred while invoking '{function}' from plugin '{plugin}': {result.LastException.InnerException.Message}");

        return result;
    }
}