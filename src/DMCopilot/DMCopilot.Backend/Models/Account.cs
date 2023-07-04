using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DMCopilot.Backend.Models
{
    /// <summary>
    /// Represents an account in the system.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the unique identifier for the account.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the account.
        /// This is used as the unique identifier and partition key for the account.
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the active tenant for the account.
        /// </summary>
        [JsonProperty(PropertyName = "activetenantid")]
        public Guid ActiveTenantId { get; set; }

        /// <summary>
        /// Gets or sets the list of Tenant Roles associated with the account.
        /// </summary>
        [JsonProperty(PropertyName = "tenantroles")]

        public List<AccountTenantRole> TenantRoles { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="id">The unique id for the account.</param>
        /// <param name="name">The name of the account.</param>
        /// <param name="email">The email address associated with the account.</param>
        /// <param name="activeTenantId">The currently active tenant.</param>
        /// <param name="tenantRoles">The roles the account has in any tenants.</param>
        public Account(Guid id, string email, string name, Guid activeTenantId, List<AccountTenantRole> tenantRoles)
        {
            Id = id;
            Email = email;
            Name = name;
            ActiveTenantId = activeTenantId;
            TenantRoles = tenantRoles;
        }
    }

    /// <summary>
    ///  The role of the account in a tenant.
    /// </summary>
    public enum TenantRole
    {
        Owner,
        Admin,
        User
    }

    public class AccountTenantRole {
        /// <summary>
        /// A role of an account has in a tenant.
        /// </summary>
        [JsonProperty(PropertyName = "tenantid")]
        public Guid TenantId { get; set; }
        
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "role")]
        public TenantRole Role { get; set; }


        public AccountTenantRole(Guid tenantId, string name, TenantRole role)
        {
            TenantId = tenantId;
            Name = name;
            Role = role;
        }
    }
}
