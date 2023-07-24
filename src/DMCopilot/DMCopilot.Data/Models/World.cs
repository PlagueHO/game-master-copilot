using Newtonsoft.Json;

namespace DMCopilot.Data.Models
{
    /// <summary>
    /// Represents a world in the application.
    /// </summary>
    public class World : IGeneratedStorageEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the unique identifier for the world.
        /// </summary>
        [JsonProperty(PropertyName = "id")] 
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the world.
        /// </summary>
        [JsonProperty(PropertyName = "tenantid")]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the world.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the world.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public String? Description { get; set; }

        /// <summary>
        /// Gets or sets the history of the world.
        /// </summary>
        [JsonProperty(PropertyName = "history")]
        public String? History { get; set; }

        /// <summary>
        /// Gets or sets a list of geographical types.
        /// </summary>
        [JsonProperty(PropertyName = "geographies")]
        public List<String> Geographies { get; set; }

        /// <summary>
        /// Gets or sets a list of climate types.
        /// </summary>
        [JsonProperty(PropertyName = "climates")]
        public List<String> Climates { get; set; }

        /// <summary>
        /// Gets or sets a dictionary of other properties for the world.
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public Dictionary<String, String>? Properties { get; set; }

        /// <summary>
        /// Information about how the storage entity was populated
        /// </summary>
        [JsonProperty(PropertyName = "generatedcontent")]
        public GeneratedContent GeneratedContent { get; set; } = new GeneratedContent();

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World(Guid id, Guid tenantId, String name, String? description = null)
        {
            Id = id;
            TenantId = tenantId;
            Name = name;
            Description = description;
        }
    }
}
