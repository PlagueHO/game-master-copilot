using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public interface ITenantStorageEntity : IStorageEntity
{
    /// <summary>
    /// Gets or sets the tenant identifier for the storage entity.
    /// </summary>
    [JsonPropertyName("tenantid")]
    public string TenantId { get; set; }
}