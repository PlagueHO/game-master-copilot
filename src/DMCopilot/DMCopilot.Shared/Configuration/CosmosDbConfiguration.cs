namespace DMCopilot.Shared.Configuration
{
    public class CosmosDbConfiguration
    {
        public Uri? EndpointUri { get; set; }
        public string DatabaseName { get; set; } = "dmcopilot";
    }
}
