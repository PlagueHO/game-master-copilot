using DMCopilot.Data.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace DMCopilot.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly string _pluginsDirectory;
        private readonly IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction> _functions;


        public SemanticKernelService(IKernel semanticKernel, SemanticKernelConfiguration configuration)
        {
            _semanticKernel = semanticKernel;

            // Set the class-level private field _skillDirectory to a value that 
            // corresponds to the value retrieved from the configuration or the default 
            // value specified in the code block, which is "\Plugins".
            var pluginsDirectory = configuration.PluginsDirectory;
            if (pluginsDirectory == null)
            {
                _pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "\\Plugins");
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
