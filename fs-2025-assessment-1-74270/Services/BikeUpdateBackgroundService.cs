using System;
using System.Threading;
using System.Threading.Tasks;
using fs_2025_assessment_1_74270.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fs_2025_assessment_1_74270.Services
{
    // Serviço de fundo que atualiza as estações periodicamente
    public class BikeUpdateBackgroundService : BackgroundService
    {
        private readonly ILogger<BikeUpdateBackgroundService> _logger;
        private readonly BikeRepository _repository;
        private readonly Random _random = new();

        public BikeUpdateBackgroundService(
            ILogger<BikeUpdateBackgroundService> logger,
            BikeRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BikeUpdateBackgroundService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // pega uma cópia da lista atual
                    var stations = _repository.GetAll().ToList();

                    foreach (var station in stations)
                    {
                        // capacidade total aleatória (entre 10 e 60, por exemplo)
                        int capacity = _random.Next(10, 61);

                        // número de bikes disponível (0 até capacidade)
                        int availableBikes = _random.Next(0, capacity + 1);

                        station.bike_stands = capacity;
                        station.available_bikes = availableBikes;
                        station.available_bike_stands = capacity - availableBikes;

                        // status simples: se não tem bike, CLOSED, senão OPEN
                        station.status = availableBikes == 0 ? "CLOSED" : "OPEN";

                        // atualiza o timestamp (epoch ms)
                        station.last_update = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                        // passa pela camada de dados (data access layer)
                        _repository.Update(station.number, station);
                    }

                    _logger.LogInformation(
                        "BikeUpdateBackgroundService atualizou {Count} estações em {Time}",
                        stations.Count,
                        DateTimeOffset.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar estações de bike.");
                }

                // intervalo aleatório entre 10 e 20 segundos
                int delaySeconds = _random.Next(10, 21);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
            }

            _logger.LogInformation("BikeUpdateBackgroundService finalizado.");
        }
    }
}
