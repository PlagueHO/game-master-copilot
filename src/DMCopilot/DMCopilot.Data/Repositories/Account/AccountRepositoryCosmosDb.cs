﻿using DMCopilot.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DMCopilot.Data.Repositories
{
    public class AccountRepositoryCosmosDb : IAccountRepository
    {
        private readonly Container _container;
        private readonly ILogger<AccountRepositoryCosmosDb> _logger;

        public AccountRepositoryCosmosDb(CosmosClient client, string databaseName, string containerName, ILogger<AccountRepositoryCosmosDb> logger)
        {
            _container = client.GetContainer(databaseName, containerName);
            _logger = logger;
            _logger.LogInformation($"Initialized {nameof(AccountRepositoryCosmosDb)} using container '{containerName}'.");
        }

        public async Task<Account> GetAccountAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Account>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new AccountNotFoundException(id);
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

        public async Task<Account> UpdateAccountAsync(string id, Account account)
        {
            account.Id = id;
            var response = await _container.UpsertItemAsync(account, new PartitionKey(id));
            return response.Resource;
        }

        public async Task<bool> DeleteAccountAsync(string id)
        {
            try
            {
                await _container.DeleteItemAsync<Account>(id, new PartitionKey(id));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }
    }
}
