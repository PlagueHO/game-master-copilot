using System.Text.Json.Serialization;

namespace GMCopilot.Core.Models;

/// <summary>
/// Enum for the different types of tenants
/// </summary>
public enum TenantType
{
    Individual,
    Organization
}

/// <summary>
/// Represents a tenant in the application.
/// </summary>
public class Tenant : IStorageEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the tenant.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the type of the tenant.
    /// </summary>
    [JsonPropertyName("type")]
    public TenantType Type { get; set; }

    /// <summary>
    /// Gets or sets the name of the tenant.
    /// This will be the name of the individual or the name of the organization.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the account of the owner of the tenant. This is the email address of the owner.
    /// </summary>
    [JsonPropertyName("owneraccount")]
    public Guid OwnerAccount { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tenant"/> class.
    /// </summary>
    public Tenant(Guid id, string name, Guid ownerAccount, TenantType type = TenantType.Individual)
    {
        Id = id;
        Type = type;
        Name = name;
        OwnerAccount = ownerAccount;
    }

    /// <summary>
    /// Outputs the name of the tenant.
    /// </summary>
    /// <returns>The name of the type and name of the tenant.</returns>
    public override string ToString()
    {
        return $"{Type}: {Name}";
    }
}