﻿using DMCopilot.Data.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace DMCopilot.Data.Repositories
{
    public class CharacterRepositoryCosmosDb : ICharacterRepository
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;
        private readonly ILogger<CharacterRepositoryCosmosDb> _logger;

        public CharacterRepositoryCosmosDb(CosmosClient cosmosClient, String databaseName, String containerName, ILogger<CharacterRepositoryCosmosDb> logger)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
            _logger.LogInformation($"Initialized {nameof(CharacterRepositoryCosmosDb)} using container '{containerName}'.");
        }

        public async Task<Character> GetCharacterAsync(Guid id)
        {
            try
            {
                var response = await _container.ReadItemAsync<Character>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Character>> GetCharactersAsync()
        {
            var query = _container.GetItemQueryIterator<Character>();
            var results = new List<Character>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Character> CreateCharacterAsync(Character character)
        {
            character.Id = Guid.NewGuid();
            var response = await _container.CreateItemAsync(character, new PartitionKey(character.Id.ToString()));
            return response.Resource;
        }

        public async Task<Character> UpdateCharacterAsync(Guid id, Character character)
        {
            var existingCharacter = await GetCharacterAsync(id);
            if (existingCharacter == null)
            {
                return null;
            }
            character.Id = existingCharacter.Id;
            var response = await _container.ReplaceItemAsync(character, existingCharacter.Id.ToString(), new PartitionKey(existingCharacter.Id.ToString()));
            return response.Resource;
        }

        public async Task<Boolean> DeleteCharacterAsync(Guid id)
        {
            var existingCharacter = await GetCharacterAsync(id);
            if (existingCharacter == null)
            {
                return false;
            }
            var response = await _container.DeleteItemAsync<Character>(id.ToString(), new PartitionKey(id.ToString()));
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}