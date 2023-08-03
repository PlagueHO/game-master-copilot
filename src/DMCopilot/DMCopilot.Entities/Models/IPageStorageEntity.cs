using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public interface IPageStorageEntity : ITypedTenantStorageEntity
{
    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonPropertyName("generatedcontent")]
    public PageGeneratedContent GeneratedContent { get; set; }
}