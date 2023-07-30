using Azure.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMCopilot.Shared.Services;

public interface IAzureCredentialService
{
    public DefaultAzureCredential GetDefaultAzureCredential();
}

