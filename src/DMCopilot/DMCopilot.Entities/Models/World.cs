using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMCopilot.Entities.Models
{
    /// <summary>
    /// Represents a world in the application.
    /// </summary>
    public class World : IPageStorageEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the world.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <inheritdoc/>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "World";

        /// <summary>
        /// Gets or sets the unique identifier for the world.
        /// </summary>
        [JsonPropertyName("tenantid")]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the world.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the world.
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the history of the world.
        /// </summary>
        [JsonPropertyName("history")]
        public string? History { get; set; }

        /// <summary>
        /// Gets or sets a list of geographical types.
        /// </summary>
        [JsonPropertyName("geographies")]
        public List<string>? Geographies { get; set; }

        /// <summary>
        /// Gets or sets a list of climate types.
        /// </summary>
        [JsonPropertyName("climates")]
        public List<string>? Climates { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of other properties for the world.
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, string>? Properties { get; set; }

        /// <summary>
        /// Information about how the storage entity was populated
        /// </summary>
        [JsonPropertyName("generatedcontent")]
        public PageGeneratedContent GeneratedContent { get; set; } = new PageGeneratedContent();

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

        /// <summary>
        /// Converts the current <see cref="World"/> object to a JSON string.
        /// </summary>
        /// <returns>A JSON formatted <see cref="string"/>.</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        /// <summary>
        /// Deserializes the provided JSON string into a new <see cref="World"/> object.
        /// </summary>
        /// <param name="json">The JSON <see cref="string"/> to deserialize.</param>
        /// <returns>A new <see cref="World"/> object based on the provided JSON string.</returns>
        public static World FromJson(string json)
        {
            return JsonSerializer.Deserialize<World>(json);
        }
    }
}