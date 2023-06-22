namespace dungeon_master_copilot_server.Data
{
    /// <summary>
    /// Represents a tenant in the application.
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Gets or sets the unique identifier for the tenant.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the contact email for the tenant.
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// </summary>
        public Tenant()
        {
        }
    }
}