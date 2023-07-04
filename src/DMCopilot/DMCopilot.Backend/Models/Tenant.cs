using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Graph;
using Newtonsoft.Json;

namespace DMCopilot.Backend.Models
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
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public TenantType Type { get; set; } = TenantType.Individual;

        /// <summary>
        /// Gets or sets the name of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the owner of the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "owneremail")]
        public string OwnerEmail { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tenant"/> class.
        /// </summary>
        public Tenant(Guid id, string name, string ownerEmail, TenantType type = TenantType.Individual)
        {
            Id = id;
            Type = type;
            Name = name;
            OwnerEmail = ownerEmail;
        }
    }
}