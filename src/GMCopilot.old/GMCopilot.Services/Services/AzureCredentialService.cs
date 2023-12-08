using Azure.Identity;
using GMCopilot.Services.Options;
using Microsoft.Extensions.Options;

namespace GMCopilot.Services;

/// <summary>
/// Service to return the DefaultAzureCredential
/// </summary>
public class AzureCredentialService
{
    public readonly AuthorizationOptions _options;
    public readonly DefaultAzureCredential _defaultAzureCredential;

    public AzureCredentialService(IOptions<AuthorizationOptions> options)
    {
        _options = options.Value;
        _defaultAzureCredential = new DefaultAzureCredential(
                       new DefaultAzureCredentialOptions
                       {
                           TenantId = _options.AzureAd.TenantId
                       });
    }

    public DefaultAzureCredential GetDefaultAzureCredential()
    {
        return _defaultAzureCredential;
    }
}