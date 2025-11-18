using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace fs_2025_assessment_1_74270.Models
{
    // ====== MODELOS ======

    public class BikeStation
    {
        public int number { get; set; }

        // id usado no Cosmos (string)
        public string id => number.ToString();

        public string contract_name { get; set; }
        public string name { get; set; }
        public string address { get; set; }

        public Position position { get; set; }

        public bool banking { get; set; }
        public bool bonus { get; set; }

        public int bike_stands { get; set; }
        public int available_bike_stands { get; set; }
        public int available_bikes { get; set; }

        public string status { get; set; }
        public long last_update { get; set; }
    }

    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    // ====== REPOSITÓRIO (CRUD em cima do ficheiro JSON) ======

    public class BikeRepository
    {
        private readonly List<BikeStation> _stations;
        private readonly string _filePath;

        public BikeRepository(IWebHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");

            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _stations = JsonSerializer.Deserialize<List<BikeStation>>(json)
                           ?? new List<BikeStation>();
            }
            else
            {
                _stations = new List<BikeStation>();
            }
        }

        // READ – lista tudo
        public List<BikeStation> GetAll() => _stations;

        // READ – 1 estação pelo número
        public BikeStation? Get(int number) =>
            _stations.FirstOrDefault(s => s.number == number);

        // CREATE
        public BikeStation Add(BikeStation station)
        {
            // se number vier 0, cria um novo id sequencial
            if (station.number == 0)
            {
                station.number = _stations.Count == 0
                    ? 1
                    : _stations.Max(s => s.number) + 1;
            }

            _stations.Add(station);
            SaveChanges(); // POST ainda grava no ficheiro
            return station;
        }

        // UPDATE
        public bool Update(int number, BikeStation updated)
        {
            var existing = Get(number);
            if (existing == null) return false;

            // mantém o mesmo number
            existing.contract_name = updated.contract_name;
            existing.name = updated.name;
            existing.address = updated.address;
            existing.position = updated.position;
            existing.banking = updated.banking;
            existing.bonus = updated.bonus;
            existing.bike_stands = updated.bike_stands;
            existing.available_bike_stands = updated.available_bike_stands;
            existing.available_bikes = updated.available_bikes;
            existing.status = updated.status;
            existing.last_update = updated.last_update;

            // ⚠ NÃO grava mais no ficheiro para evitar conflito com o background service
            // SaveChanges();

            return true;
        }

        // DELETE
        public bool Delete(int number)
        {
            var existing = Get(number);
            if (existing == null) return false;

            _stations.Remove(existing);
            SaveChanges(); // se quiser, pode comentar também se não precisar persistir deletar
            return true;
        }

        // grava no ficheiro JSON (usado por Add e Delete)
        private void SaveChanges()
        {
            var json = JsonSerializer.Serialize(
                _stations,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_filePath, json);
        }
    }
}
