using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models;

public interface IStorageEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the storage entity.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
}
