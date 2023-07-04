using System.Collections.Generic;

namespace DMCopilot.Backend.Models
{
    /// <summary>
    /// Represents an account in the system.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the email address associated with the account.
        /// This is used as the unique identifier and partition key for the account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the account.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the list of Tenant Roles associated with the account.
        /// </summary>
        public List<AccountTenantRole>? TenantRoles { get; set; }

        /// <summary>
        /// Gets or sets the active tenant for the account.
        /// </summary>
        public Guid? ActiveTenantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        /// <param name="name">The name of the account.</param>
        /// <param name="email">The email address associated with the account.</param>
        /// <param name="activeTenantId">The currently active tenant.</param>
        /// <param name="tenantRoles">The roles the account has in any tenants.</param>
        public Account(string email, string ? name = null, Guid? activeTenantId = null, List<AccountTenantRole>? tenantRoles = null)
        {
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
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public TenantRole Role { get; set; }
        public AccountTenantRole(Guid tenantId, string name, TenantRole role)
        {
            TenantId = tenantId;
            Name = name;
            Role = role;
        }
    }
}
