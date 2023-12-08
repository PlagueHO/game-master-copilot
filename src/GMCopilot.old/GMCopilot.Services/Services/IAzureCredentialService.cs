using Azure.Identity;

namespace GMCopilot.Services;

public interface IAzureCredentialService
{
    public DefaultAzureCredential GetDefaultAzureCredential();
}