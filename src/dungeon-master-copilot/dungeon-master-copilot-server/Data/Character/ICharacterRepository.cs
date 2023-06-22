using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dungeon_master_copilot_server.Data.Character
{
    public interface ICharacterRepository
    {
        Task<CharacterData> GetCharacterAsync(Guid id);
        Task<IEnumerable<CharacterData>> GetCharactersAsync();
        Task<CharacterData> CreateCharacterAsync(CharacterData characterData);
        Task<CharacterData> UpdateCharacterAsync(Guid id, CharacterData characterData);
        Task<bool> DeleteCharacterAsync(Guid id);
    }
}
