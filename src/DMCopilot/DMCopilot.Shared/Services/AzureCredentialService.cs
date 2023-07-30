using Azure.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DMCopilot.Shared.Services.Options;

namespace DMCopilot.Shared.Services;

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

