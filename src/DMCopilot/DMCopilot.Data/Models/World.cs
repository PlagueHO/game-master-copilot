using Newtonsoft.Json;

namespace DMCopilot.Data.Models;

/// <summary>
/// Represents a world in the application.
/// </summary>
public class World : IGeneratedStorageEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the world.
    /// </summary>
    [JsonProperty(PropertyName = "id")] 
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the world.
    /// </summary>
    [JsonProperty(PropertyName = "tenantid")]
    public string TenantId { get; set; }

    /// <summary>
    /// Gets or sets the name of the world.
    /// </summary>
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the world.
    /// </summary>
    [JsonProperty(PropertyName = "description")]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the history of the world.
    /// </summary>
    [JsonProperty(PropertyName = "history")]
    public string? History { get; set; }

    /// <summary>
    /// Gets or sets a list of geographical types.
    /// </summary>
    [JsonProperty(PropertyName = "geographies")]
    public List<string> Geographies { get; set; }

    /// <summary>
    /// Gets or sets a list of climate types.
    /// </summary>
    [JsonProperty(PropertyName = "climates")]
    public List<string> Climates { get; set; }

    /// <summary>
    /// Gets or sets a dictionary of other properties for the world.
    /// </summary>
    [JsonProperty(PropertyName = "properties")]
    public Dictionary<string, string>? Properties { get; set; }

    /// <summary>
    /// Information about how the storage entity was populated
    /// </summary>
    [JsonProperty(PropertyName = "generatedcontent")]
    public GeneratedContent GeneratedContent { get; set; } = new GeneratedContent();

    /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class.
    /// </summary>
    public World(string id, string tenantId, string name, string? description = null)
    {
        Id = id;
        TenantId = tenantId;
        Name = name;
        Description = description;
    }
}
