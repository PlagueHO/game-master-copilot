using DMCopilot.Data.Models;

namespace DMCopilot.Data.Repositories;

public interface ICharacterRepository
{
    Task<Character> GetCharacterAsync(string id);
    Task<IEnumerable<Character>> GetCharactersAsync();
    Task<Character> CreateCharacterAsync(Character character);
    Task<Character> UpdateCharacterAsync(string id, Character character);
    Task<bool> DeleteCharacterAsync(string id);
}
