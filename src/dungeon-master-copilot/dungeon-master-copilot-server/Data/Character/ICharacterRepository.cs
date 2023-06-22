using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dungeon_master_copilot_server.Data
{
    public interface ICharacterRepository
    {
        Task<Character> GetCharacterAsync(Guid id);
        Task<IEnumerable<Character>> GetCharactersAsync();
        Task<Character> CreateCharacterAsync(Character character);
        Task<Character> UpdateCharacterAsync(Guid id, Character character);
        Task<bool> DeleteCharacterAsync(Guid id);
    }
}
