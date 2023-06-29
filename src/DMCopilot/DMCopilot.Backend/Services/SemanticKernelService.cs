using Azure.Identity;
using DMCopilot.Backend.Data.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace DMCopilot.Backend.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly SemanticKernelConfiguration _semanticKernelConfiguration;
        private readonly DefaultAzureCredential _azureCredential;
        private readonly string _pluginsDirectory;
        private readonly IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction> _functions;


        public SemanticKernelService(DefaultAzureCredential azureCredential, SemanticKernelConfiguration semanticKernelConfiguration)
        {
            Console.WriteLine("Creating Semantic Kernel");

            _azureCredential = azureCredential;
            _semanticKernelConfiguration = semanticKernelConfiguration;
            var semanticKernelBuilder = new KernelBuilder();

            var serviceActions = new Dictionary<SemanticKernelConfigurationServiceType, Action<SemanticKernelConfigurationService>>()
            {
                { SemanticKernelConfigurationServiceType.AzureOpenAIServiceTextCompletion, (service) => semanticKernelBuilder.WithAzureTextCompletionService(service.Deployment,
                                                                                                                                    service.Endpoint,
                                                                                                                                    azureCredential,
                                                                                                                                    service.Id) },
                { SemanticKernelConfigurationServiceType.AzureOpenAIServiceChatCompletion, (service) => semanticKernelBuilder.WithAzureTextCompletionService(service.Deployment,
                                                                                                                                    service.Endpoint,
                                                                                                                                    azureCredential,
                                                                                                                                    service.Id) },
                { SemanticKernelConfigurationServiceType.AzureOpenAIServiceEmbedding, (service) => semanticKernelBuilder.WithAzureTextEmbeddingGenerationService(service.Deployment,
                                                                                                                                         service.Endpoint,
                                                                                                                                         azureCredential,
                                                                                                                                         service.Id) }
            };

            if (semanticKernelConfiguration.Services == null)
            {
                throw new ArgumentException("Semantic Kernel configuration services are null");
            }

            foreach (var service in semanticKernelConfiguration.Services)
            {
                Console.WriteLine($"Adding service {service.Id} using deployment {service.Deployment} on endpoint {service.Endpoint} to Semantic Kernel");

                if (serviceActions.TryGetValue(service.Type, out var action))
                {
                    action(service);
                }
                else
                {
                    throw new ArgumentException("Invalid Semantic Kernel service type");
                }
            }

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

            // Import a Semantic Skill from the specified directory and output a console message to confirm it.
            _functions = _semanticKernel.ImportSemanticSkillFromDirectory(_pluginsDirectory, "CharacterPlugin");
            Console.WriteLine($"Imported {_functions.Count()} Semantic Skill from '{_pluginsDirectory}'.");
        }

        public async Task<SKContext> InvokeFunctionAsync(string function, string input)
        {
            var context = _semanticKernel.CreateNewContext();
            context["input"] = input;
            var result = await _functions[function].InvokeAsync(context);
            return result;
        }
    }
}
