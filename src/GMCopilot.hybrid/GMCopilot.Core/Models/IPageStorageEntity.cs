using System.Text.Json.Serialization;

namespace GMCopilot.Core.Models;

public interface IPageStorageEntity : IStorageTenantedTypedEntity
{
    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonPropertyName("generatedcontent")]
    public PageGeneratedContent GeneratedContent { get; set; }
}