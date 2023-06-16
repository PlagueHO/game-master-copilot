using Microsoft.IdentityModel.Protocols;
using Microsoft.SemanticKernel;

namespace dungeon_master_copilot_server.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly string _skillDirectory = "\\Skills";
        private readonly IDictionary<string, Microsoft.SemanticKernel.SkillDefinition.ISKFunction> _skills;

        public SemanticKernelService(IKernel semanticKernel, IConfiguration configuration)
        {
            _semanticKernel = semanticKernel;

            // Set the class-level private field _skillDirectory to a value that 
            // corresponds to the value retrieved from the configuration or the default 
            // value specified in the code block, which is "\Skills".
            var skillDirectory = configuration["SemanticKernel:SkillsDirectory"];
            if (skillDirectory != null)
                _skillDirectory = skillDirectory;

            // Import a Semantic Skill from the specified directory and output a console message to confirm it.
            _skills = _semanticKernel.ImportSemanticSkillFromDirectory(_skillDirectory);
            Console.WriteLine($"Imported {_skills.Count()} Semantic Skill from {_skillDirectory}.");
        }
    }
}
