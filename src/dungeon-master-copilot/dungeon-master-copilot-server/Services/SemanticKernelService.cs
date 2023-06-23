using dungeon_master_copilot_server.Data.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace dungeon_master_copilot_server.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly string _pluginsDirectory = "\\Plugins";
        private readonly IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction> _skills;

        public SemanticKernelService(IKernel semanticKernel, SemanticKernelConfiguration configuration)
        {
            _semanticKernel = semanticKernel;

            // Set the class-level private field _skillDirectory to a value that 
            // corresponds to the value retrieved from the configuration or the default 
            // value specified in the code block, which is "\Skills".
            var pluginsDirectory = configuration.PluginsDirectory;
            if (pluginsDirectory != null)
                _pluginsDirectory = pluginsDirectory;

            // Import a Semantic Skill from the specified directory and output a console message to confirm it.
            _skills = _semanticKernel.ImportSemanticSkillFromDirectory(_pluginsDirectory);
            Console.WriteLine($"Imported {_skills.Count()} Semantic Skill from {_pluginsDirectory}.");
        }

        public async Task<SKContext> InvokeSkillAsync(string skill, string input)
        {
            if (_semanticKernel.Skills.TryGetFunction(skill, out ISKFunction? function))
            {
                var context = _semanticKernel.CreateNewContext();
                context["input"] = input;
                var result = await function.InvokeAsync(context);
                return result;
            }
            else
            {
                throw new Exception($"Skill {skill} not found.");
            }
            // Process the input using the Semantic Kernel and output the result.
        }
    }
}
