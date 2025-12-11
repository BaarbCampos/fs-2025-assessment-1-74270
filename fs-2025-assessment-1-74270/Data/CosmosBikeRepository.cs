using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using fs_2025_assessment_1_74270.Models;

namespace fs_2025_assessment_1_74270.Data
{
    public class CosmosBikeRepository
    {
        private readonly Container _container;

        public CosmosBikeRepository(CosmosClient client, IConfiguration config)
        {
            // Usa a secção "CosmosDb" do appsettings.json
            var dbName = config["CosmosDb:DatabaseName"];
            var containerName = config["CosmosDb:ContainerName"];

            _container = client.GetContainer(dbName, containerName);
        }

        // ================================================================
        // GET ALL
        // ================================================================
        public async Task<List<BikeStation>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<BikeStation>(
                new QueryDefinition("SELECT * FROM c"));

            var results = new List<BikeStation>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        // ================================================================
        // GET ONE
        // ================================================================
        public async Task<BikeStation?> GetByNumberAsync(int number)
        {
            var sql = "SELECT * FROM c WHERE c.number = @n";

            var iterator = _container.GetItemQueryIterator<BikeStation>(
                new QueryDefinition(sql).WithParameter("@n", number));

            if (!iterator.HasMoreResults) return null;

            var page = await iterator.ReadNextAsync();
            return page.FirstOrDefault();
        }

        // ================================================================
        // CREATE
        // ================================================================
        public async Task<BikeStation> AddAsync(BikeStation station)
        {
            // se não vier contract_name, usa "dublin"
            var pk = new PartitionKey(station.contract_name ?? "dublin");

            var response = await _container.CreateItemAsync(station, pk);
            return response.Resource;
        }

        // ================================================================
        // UPDATE
        // ================================================================
        public async Task<bool> UpdateAsync(int number, BikeStation station)
        {
            station.number = number;

            var pk = new PartitionKey(station.contract_name ?? "dublin");

            var response = await _container.UpsertItemAsync(station, pk);

            return response.StatusCode == HttpStatusCode.OK ||
                   response.StatusCode == HttpStatusCode.Created;
        }

        // ================================================================
        // DELETE
        // ================================================================
        public async Task<bool> DeleteAsync(int number)
        {
            var existing = await GetByNumberAsync(number);
            if (existing == null) return false;

            var pk = new PartitionKey(existing.contract_name ?? "dublin");

            var response = await _container.DeleteItemAsync<BikeStation>(existing.id, pk);

            return response.StatusCode == HttpStatusCode.NoContent ||
                   response.StatusCode == HttpStatusCode.OK;
        }
    }
}
