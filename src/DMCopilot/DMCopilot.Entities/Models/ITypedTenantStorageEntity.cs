using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public interface ITypedTenantStorageEntity : ITenantStorageEntity
{
    /// <summary>
    /// Gets or sets the ty[e for the storage entity.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

}