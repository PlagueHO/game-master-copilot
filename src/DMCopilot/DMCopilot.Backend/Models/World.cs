using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace DMCopilot.Backend.Models
{
    /// <summary>
    /// Represents a world in the application.
    /// </summary>
    public class World
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
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the world.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string? Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World(Guid id, Guid tenantId, string name, string? description = null)
        {
            Id = id;
            TenantId = tenantId;
            Name = name;
            Description = description;
        }
    }
}
