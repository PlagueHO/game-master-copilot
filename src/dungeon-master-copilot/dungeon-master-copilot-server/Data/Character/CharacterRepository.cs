using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace dungeon_master_copilot_server.Data.Character
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly Microsoft.Azure.Cosmos.Container _container;

        public CharacterRepository(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task<CharacterData> GetCharacterAsync(Guid id)
        {
            try
            {
                var response = await _container.ReadItemAsync<CharacterData>(id.ToString(), new PartitionKey(id.ToString()));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CharacterData>> GetCharactersAsync()
        {
            var query = _container.GetItemQueryIterator<CharacterData>();
            var results = new List<CharacterData>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<CharacterData> CreateCharacterAsync(CharacterData characterData)
        {
            characterData.Id = Guid.NewGuid();
            var response = await _container.CreateItemAsync(characterData, new PartitionKey(characterData.Id.ToString()));
            return response.Resource;
        }

        public async Task<CharacterData> UpdateCharacterAsync(Guid id, CharacterData characterData)
        {
            var existingCharacter = await GetCharacterAsync(id);
            if (existingCharacter == null)
            {
                return null;
            }
            characterData.Id = existingCharacter.Id;
            var response = await _container.ReplaceItemAsync(characterData, existingCharacter.Id.ToString(), new PartitionKey(existingCharacter.Id.ToString()));
            return response.Resource;
        }

        public async Task<bool> DeleteCharacterAsync(Guid id)
        {
            var existingCharacter = await GetCharacterAsync(id);
            if (existingCharacter == null)
            {
                return false;
            }
            var response = await _container.DeleteItemAsync<CharacterData>(id.ToString(), new PartitionKey(id.ToString()));
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
    }
}