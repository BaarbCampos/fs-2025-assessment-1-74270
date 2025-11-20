using fs_2025_assessment_1_74270;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace fs_2025_assessment_1_74270.Tests.EndpointTests
{
    public class StationsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StationsEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetStations_ReturnsOk()
        {
            var client = _factory.CreateClient();

            // Se sua rota principal for v2, troque para /api/v2/stations
            var response = await client.GetAsync("/api/v1/stations");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(content));
        }
    }
}
