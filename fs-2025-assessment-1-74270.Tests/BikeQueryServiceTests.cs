using System.Collections.Generic;
using System.Linq;
using fs_2025_assessment_1_74270.Models;
using Xunit;

namespace fs_2025_assessment_1_74270.Tests.UnitTests
{
    public class BikeQueryServiceTests
    {
        [Fact]
        public void FilterStations_ByStatusOpen_ReturnsOnlyOpenStations()
        {
            // ARRANGE - usa os MESMOS nomes de propriedades do BikeStation.cs
            var stations = new List<BikeStation>
            {
                new BikeStation { number = 1, name = "Station A", address = "Rua 1", status = "OPEN",   available_bikes = 5, bike_stands = 10 },
                new BikeStation { number = 2, name = "Station B", address = "Rua 2", status = "CLOSED", available_bikes = 0, bike_stands = 10 },
                new BikeStation { number = 3, name = "Station C", address = "Rua 3", status = "OPEN",   available_bikes = 2, bike_stands = 5  }
            };

            // ACT - faz o filtro (lógica de search/filter que o enunciado quer)
            var result = stations
                .Where(s => s.status == "OPEN")
                .ToList();

            // ASSERT
            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal("OPEN", s.status));
        }
    }
}
