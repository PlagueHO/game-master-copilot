using Azure.Identity;

namespace DMCopilot.Services;

public interface IAzureCredentialService
{
    public DefaultAzureCredential GetDefaultAzureCredential();
}