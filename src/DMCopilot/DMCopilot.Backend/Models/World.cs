using Microsoft.Graph;

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
        public Guid? WorldId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the world.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the name of the world.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the world.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="World"/> class.
        /// </summary>
        public World(Guid? worldId, Guid? tenantId = null, string? name = null, string? description = null)
        {
            WorldId = worldId;
            TenantId = tenantId;
            Name = name;
            Description = description;
        }
    }
}
