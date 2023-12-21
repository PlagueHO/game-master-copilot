using System.Text.Json.Serialization;

namespace GMCopilot.Core.Models;

/// <summary>
/// Represents an account in the system.
/// </summary>
public class Account : IStorageEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the account.
    /// This will be the email address of the user.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the account.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the active tenant for the account.
    /// </summary>
    [JsonPropertyName("activetenantid")]
    public string ActiveTenantId { get; set; }

    /// <summary>
    /// Gets or sets the list of Tenant Roles associated with the account.
    /// </summary>
    [JsonPropertyName("tenantroles")]
    public List<AccountTenantRole> TenantRoles { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Account"/> class.
    /// </summary>
    /// <param name="id">The unique id for the account.</param>
    /// <param name="name">The name of the account.</param>
    /// <param name="email">The email address associated with the account.</param>
    /// <param name="activeTenantId">The currently active tenant.</param>
    /// <param name="tenantRoles">The roles the account has in any tenants.</param>
    public Account(string id, string name, string activeTenantId, List<AccountTenantRole> tenantRoles)
    {
        Id = id;
        Name = name;
        ActiveTenantId = activeTenantId;
        TenantRoles = tenantRoles;
    }

    /// <summary>
    /// Adds a tenant role to the account.
    /// </summary>
    /// <param name="tenantRole">The tenant role to add.</param>
    public void AddTenantRole(AccountTenantRole tenantRole)
    {
        TenantRoles.Add(tenantRole);
    }

    /// <summary>
    /// Returns the string representation of the account.
    /// </summary>
    /// <returns>The name of the account.</returns>
    public override string ToString()
    {
        return Name;
    }
}

/// <summary>
///  The role of the account in a tenant.
/// </summary>
public enum TenantRole
{
    Owner,
    Admin,
    Contributor,
    Reader
}

/// <summary>
/// Represents the tenants that the account has access to and the role they have in the tenant.
/// </summary>
public class AccountTenantRole
{
    [JsonPropertyName("tenantid")]
    public string TenantId { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public TenantType Type { get; set; }

    [JsonPropertyName("role")]
    public TenantRole Role { get; set; }

    public AccountTenantRole(string tenantId, string email, string name, TenantType type, TenantRole role)
    {
        TenantId = tenantId;
        Email = email;
        Name = name;
        Type = type;
        Role = role;
    }
}