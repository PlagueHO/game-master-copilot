using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DMCopilot.Backend.Models;

namespace DMCopilot.Backend.Data
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
