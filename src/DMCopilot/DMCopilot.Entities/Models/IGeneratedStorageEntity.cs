using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public interface IGeneratedStorageEntity : IStorageEntity
{
    /// <summary>
    /// Gets or sets the generated entity type
    /// </summary>
    [JsonPropertyName("type")]
    public GeneratedStorageEntityType Type { get; }

    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonPropertyName("generatedcontent")]
    public GeneratedContent GeneratedContent { get; set; }
}