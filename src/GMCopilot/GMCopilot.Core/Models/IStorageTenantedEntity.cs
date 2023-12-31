using System.Text.Json.Serialization;

namespace GMCopilot.Core.Models;

public interface IStorageTenantedEntity : IStorageEntity
{
    /// <summary>
    /// Gets or sets the tenant identifier for the storage entity.
    /// </summary>
    [JsonPropertyName("tenantid")]
    public Guid TenantId { get; set; }
}