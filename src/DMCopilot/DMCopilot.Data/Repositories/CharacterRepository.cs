using DMCopilot.Entities.Models;

namespace DMCopilot.Data.Repositories;

public class CharacterRepositoryNew : Repository<Character>
{
    /// <summary>
    /// Initializes a new instance of the CharacterRepository class.
    /// </summary>
    /// <param name="storageContext">The storage context.</param>
    public CharacterRepositoryNew(IStorageContext<Character> storageContext)
        : base(storageContext)
    {
    }
}