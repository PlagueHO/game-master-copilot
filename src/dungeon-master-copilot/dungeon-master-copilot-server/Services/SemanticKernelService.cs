using Microsoft.IdentityModel.Protocols;
using Microsoft.SemanticKernel;

namespace dungeon_master_copilot_server.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly IKernel _semanticKernel;
        private readonly string _skillDirectory = "\\Skills";

        public SemanticKernelService(IKernel semanticKernel, IConfiguration configuration)
        {
            _semanticKernel = semanticKernel;
            var skillDirectory = configuration["SemanticKernel:SkillsDirectory"];
            if(skillDirectory != null)
                _skillDirectory = skillDirectory;
            _semanticKernel.ImportSemanticSkillFromDirectory(_skillDirectory);
        }
    }
}
