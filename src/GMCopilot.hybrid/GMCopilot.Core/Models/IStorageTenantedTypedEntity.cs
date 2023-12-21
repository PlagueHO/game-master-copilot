using System.Text.Json.Serialization;
using GMCopilot.Core.Types;

namespace GMCopilot.Core.Models;

public interface IStorageTenantedTypedEntity : IStorageTenantedEntity
{
    /// <summary>
    /// Gets or sets the ty[e for the storage entity.
    /// </summary>
    [JsonPropertyName("type")]
    public PageStorageEntityTypes Type { get; set; }

}