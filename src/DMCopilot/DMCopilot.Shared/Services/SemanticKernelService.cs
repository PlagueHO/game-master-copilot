using Azure.Identity;
using DMCopilot.Shared.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using System.Configuration;

namespace DMCopilot.Shared.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly ILogger<SemanticKernelService> _logger;
        private readonly SemanticKernelConfiguration _semanticKernelConfiguration;
        private readonly DefaultAzureCredential? _azureCredential;
        private readonly string? _azureOpenAiApiKey;
        private readonly string _pluginsDirectory;
        private IDictionary<string, IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction>> _functions;

        public SemanticKernelService(SemanticKernelConfiguration semanticKernelConfiguration, ILogger<SemanticKernelService> logger)
        {
            _semanticKernelConfiguration = semanticKernelConfiguration;
            _azureOpenAiApiKey = semanticKernelConfiguration.AzureOpenAiApiKey;
            _logger = logger;

            _logger.LogInformation("Creating Semantic Kernel");

            var semanticKernelBuilder = new KernelBuilder();

            // Load the generative AI services that will be available to Semantic Kernel
            LoadAzureOpenAiTextCompletionServices(semanticKernelBuilder);
            LoadAzureOpenAiChatCompletionServices(semanticKernelBuilder);
            LoadAzureOpenAiTextEmbeddingGenerationServices(semanticKernelBuilder);
            LoadAzureOpenAIImageGenerationServices(semanticKernelBuilder);

            _semanticKernel = semanticKernelBuilder.Build();

            // Set the class-level private field _skillDirectory to a value that 
            // corresponds to the value retrieved from the configuration or the default 
            // value specified in the code block, which is "\Plugins".
            var pluginsDirectory = semanticKernelConfiguration.PluginsDirectory;
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

        public SemanticKernelService(DefaultAzureCredential azureCredential, SemanticKernelConfiguration semanticKernelConfiguration, ILogger<SemanticKernelService> logger) : this(semanticKernelConfiguration, logger)
        {
            _azureCredential = azureCredential;
        }

        public void LoadAzureOpenAiTextCompletionServices(KernelBuilder semanticKernelBuilder)
        {
            if (_semanticKernelConfiguration.AzureOpenAiTextCompletionServices == null)
                return;

            foreach (SemanticKernelAzureOpenAiTextCompletionService service in _semanticKernelConfiguration.AzureOpenAiTextCompletionServices)
            {
                if (service.Deployment == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Text Completion service is empty");

                if (service.Endpoint == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Text Completion service is empty");

                _logger.LogInformation($"Adding Azure OpenAI Text Completion service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

                if (_azureOpenAiApiKey == null)
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
                       apiKey: _azureOpenAiApiKey,
                       serviceId: service.Id,
                       setAsDefault: service.SetAsDefault);
            }
        }

        public void LoadAzureOpenAiChatCompletionServices(KernelBuilder semanticKernelBuilder)
        {
            if (_semanticKernelConfiguration.AzureOpenAiChatCompletionServices == null)
                return;

            foreach (SemanticKernelAzureOpenAiChatCompletionService service in _semanticKernelConfiguration.AzureOpenAiChatCompletionServices)
            {
                if (service.Deployment == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Chat Completion service is empty");

                if (service.Endpoint == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Chat Completion service is empty");

                _logger.LogInformation($"Adding Azure OpenAI Chat Completion service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

                if (_azureOpenAiApiKey == null)
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
                        apiKey: _azureOpenAiApiKey,
                        serviceId: service.Id,
                        setAsDefault: service.SetAsDefault,
                        alsoAsTextCompletion: service.AlsoAsTextCompletion);
            }
        }

        public void LoadAzureOpenAiTextEmbeddingGenerationServices(KernelBuilder semanticKernelBuilder)
        {
            if (_semanticKernelConfiguration.AzureOpenAiTextEmbeddingGenerationServices == null)
                return;

            foreach (SemanticKernelAzureOpenAiTextEmbeddingGenerationService service in _semanticKernelConfiguration.AzureOpenAiTextEmbeddingGenerationServices)
            {
                if (service.Deployment == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Text Embedding Generation service is empty");

                if (service.Endpoint == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Text Embedding Generation service is empty");

                _logger.LogInformation($"Adding Azure OpenAI Text Embedding Generation service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

                if (_azureOpenAiApiKey == null)
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
                        apiKey: _azureOpenAiApiKey,
                        serviceId: service.Id,
                        setAsDefault: service.SetAsDefault);
            }
        }

        public void LoadAzureOpenAIImageGenerationServices(KernelBuilder semanticKernelBuilder)
        {
            if (_semanticKernelConfiguration.AzureOpenAiImageGenerationServices == null)
                return;

            foreach (SemanticKernelAzureOpenAiImageGenerationService service in _semanticKernelConfiguration.AzureOpenAiImageGenerationServices)
            {
                if (service.Deployment == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Image Generation service is empty");

                if (service.Endpoint == null)
                    throw new ConfigurationErrorsException("Deployment setting for Azure OpenAI Image Generation service is empty");

                _logger.LogInformation($"Adding Azure OpenAI Image Generation service '{service.Id}' using deployment '{service.Deployment}' on endpoint '{service.Endpoint}' to Semantic Kernel");

                if (_azureOpenAiApiKey == null)
                    _logger.LogWarning("AzureOpenAiApiKey can not be empty if Azure OpenAI Image Generation Service is being used. Image Generation service will not be available.");

                semanticKernelBuilder.WithAzureOpenAIImageGenerationService(
                   endpoint: service.Endpoint,
                   apiKey: _azureOpenAiApiKey,
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

        public async Task<SKContext> InvokePluginFunctionAsync(string plugin, string function, Dictionary<String, String> inputs)
        {
            LoadPlugin(plugin);
            var context = _semanticKernel.CreateNewContext();
            foreach (String input in inputs.Keys)
                context[input] = inputs[input];
            _logger.LogInformation($"Invoking '{function}' from plugin '{plugin}.");
            var result = await _functions[plugin][function].InvokeAsync(context);
            return result;
        }
    }
}
