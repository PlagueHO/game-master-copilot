using DMCopilot.Backend.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Graph;

namespace DMCopilot.Backend.Data
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Container _container;
        private readonly ILogger<AccountRepository> _logger;

        public AccountRepository(CosmosClient client, string databaseName, string containerName, ILogger<AccountRepository> logger)
        {
            _container = client.GetContainer(databaseName, containerName);
            _logger = logger;
            _logger.LogInformation($"Initialized {nameof(AccountRepository)} using container '{containerName}'.");
        }

        public async Task<Account> GetAccountAsync(Guid id)
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c WHERE c.Id = @id");
            return await GetAccountByQueryAsync(queryDefinition);
        }

        public async Task<Account> GetAccountByEmailAsync(string email)
        {
            try
            {
                var response = await _container.ReadItemAsync<Account>(email, new PartitionKey(email));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new AccountNotFoundException($"Account '{email}' not found.");
            }
        }

        private async Task<Account> GetAccountByQueryAsync(QueryDefinition queryDefinition)
        {
            var query = _container.GetItemQueryIterator<Account>(queryDefinition);
            if (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                if (response != null && response.Count > 0)
                {
                    return response.FirstOrDefault();
                }
            }
            return null;
        }

        private async Task<IEnumerable<Account>> GetAccountsByQueryAsync(QueryDefinition queryDefinition)
        {
            var query = _container.GetItemQueryIterator<Account>(queryDefinition);
            var results = new List<Account>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync()
        {
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            return await GetAccountsByQueryAsync(queryDefinition);
        }

        public async Task<Account> CreateAccountAsync(Account account)
        {
            var response = await _container.CreateItemAsync(account);
            return response.Resource;
        }

        public async Task<Account> UpdateAccountAsync(string email, Account account)
        {
            account.Email = email;
            var response = await _container.UpsertItemAsync(account, new PartitionKey(email));
            return response.Resource;
        }

        public async Task<bool> DeleteAccountAsync(string email)
        {
            try
            {
                await _container.DeleteItemAsync<Account>(email, new PartitionKey(email));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}
