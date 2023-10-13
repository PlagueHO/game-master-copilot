using System.Text.Json.Serialization;

namespace GMCopilot.Entities.Models;

public interface IStorageTenantedTypedEntity : IStorageTenantedEntity
{
    /// <summary>
    /// Gets or sets the ty[e for the storage entity.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

}