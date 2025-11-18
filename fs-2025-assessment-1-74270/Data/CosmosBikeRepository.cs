using Microsoft.Azure.Cosmos;
using fs_2025_assessment_1_74270.Models;

namespace fs_2025_assessment_1_74270.Data
{
    public class CosmosBikeRepository
    {
        private readonly CosmosClient _client;
        private readonly Container _container;
        private readonly string _databaseName;
        private readonly string _containerName;

        public CosmosBikeRepository(IConfiguration configuration)
        {
            var endpoint = configuration["CosmosDb:AccountEndpoint"];
            var key = configuration["CosmosDb:AccountKey"];

            _databaseName = configuration["CosmosDb:DatabaseName"];
            _containerName = configuration["CosmosDb:ContainerName"];

            // Criar o client usando endpoint + key
            _client = new CosmosClient(endpoint, key, new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Direct
            });

            _container = _client.GetContainer(_databaseName, _containerName);
        }

        // -------- GET ALL (usado pelo V2) --------
        public async Task<List<BikeStation>> GetAllAsync()
        {
            var query = _container.GetItemQueryIterator<BikeStation>(
                new QueryDefinition("SELECT * FROM c"));

            var results = new List<BikeStation>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        // -------- GET BY NUMBER --------
        public async Task<BikeStation?> GetByNumberAsync(int number)
        {
            var query = _container.GetItemQueryIterator<BikeStation>(
                new QueryDefinition("SELECT * FROM c WHERE c.number = @number")
                    .WithParameter("@number", number));

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                var item = response.FirstOrDefault();
                if (item != null) return item;
            }

            return null;
        }
    }
}
