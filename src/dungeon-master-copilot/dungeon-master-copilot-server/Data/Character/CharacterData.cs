using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Graph;

namespace dungeon_master_copilot_server.Data.Character
{
    public class CharacterData
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? Class { get; set; }
        public string? Race { get; set; }
        public Height? Height { get; set; }
        public Weight? Weight { get; set; }
        public string? PhysicalCharacteristics { get; set; }
        public string? Voice { get; set; }
        public string? Clothing { get; set; }
        public string? PersonalityTraits { get; set; }
        public string? Ideals { get; set; }
        public string? Bonds { get; set; }
        public string? Flaws { get; set; }
    }
}
