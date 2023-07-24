using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public interface IGeneratedStorageEntity : IStorageEntity
{
    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonPropertyName("generatedcontent")]
    public GeneratedContent GeneratedContent { get; set; }
}
