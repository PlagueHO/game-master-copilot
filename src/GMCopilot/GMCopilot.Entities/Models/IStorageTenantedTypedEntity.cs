using System.Text.Json.Serialization;
using GMCopilot.Entities.Types;

namespace GMCopilot.Entities.Models;

public interface IStorageTenantedTypedEntity : IStorageTenantedEntity
{
    /// <summary>
    /// Gets or sets the ty[e for the storage entity.
    /// </summary>
    [JsonPropertyName("type")]
    public PageStorageEntityTypes Type { get; set; }

}