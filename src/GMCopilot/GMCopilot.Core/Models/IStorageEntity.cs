using System.Text.Json.Serialization;

namespace GMCopilot.Core.Models;

public interface IStorageEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the storage entity.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}