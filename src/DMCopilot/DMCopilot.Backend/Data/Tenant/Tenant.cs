namespace DMCopilot.Backend.Data
{
    // Enum for the different types of tenants
    public enum TenantType
    {
        // The tenant is for an individual
        Individual,
        // The tenant is for an organization
        Organization
    }

    /// <summary>
    /// Represents a tenant in the application.
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Gets or sets the unique identifier for the tenant.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the type of the tenant.
        /// </summary>
        public TenantType? Type { get; set; } = TenantType.Individual;

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the owner of the tenant.
        /// </summary>
        public string? OwnerEmail { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// </summary>
        public Tenant()
        {
        }
    }
}