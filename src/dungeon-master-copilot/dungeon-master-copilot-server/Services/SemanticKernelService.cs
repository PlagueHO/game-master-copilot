using Microsoft.SemanticKernel;

namespace dungeon_master_copilot_server.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly string _skillDirectory = "\\Skills";

        public SemanticKernelService(IKernel semanticKernel, string? skillDirectory)
        {
            _semanticKernel = semanticKernel;
            if (skillDirectory != null)
                _skillDirectory = skillDirectory;
            _semanticKernel.ImportSemanticSkillFromDirectory(_skillDirectory);
        }
    }
}
