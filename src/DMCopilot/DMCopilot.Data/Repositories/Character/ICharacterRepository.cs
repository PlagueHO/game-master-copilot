using DMCopilot.Data.Models;

namespace DMCopilot.Data.Repositories
{
    public interface ICharacterRepository
    {
        Task<Character> GetCharacterAsync(Guid id);
        Task<IEnumerable<Character>> GetCharactersAsync();
        Task<Character> CreateCharacterAsync(Character character);
        Task<Character> UpdateCharacterAsync(Guid id, Character character);
        Task<Boolean> DeleteCharacterAsync(Guid id);
    }
}
