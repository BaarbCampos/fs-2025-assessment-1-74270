using fs_2025_assessment_1_74270.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace fs_2025_assessment_1_74270.BlazorClient.Services
{
    public class StationsApiClient
    {
        private readonly HttpClient _http;

        public StationsApiClient(HttpClient http)
        {
            _http = http;
        }

        // GET list (with filters + paging)
        public async Task<List<BikeStation>> GetStationsAsync(
            string? q,
            string? status,
            int? minBikes,
            string? sort,
            string? dir,
            int page,
            int pageSize)
        {
            var url =
                $"/api/v1/stations" +
                $"?q={q}" +
                $"&status={status}" +
                $"&minBikes={minBikes}" +
                $"&sort={sort}" +
                $"&dir={dir}" +
                $"&page={page}" +
                $"&pageSize={pageSize}";

            var result = await _http.GetFromJsonAsync<List<BikeStation>>(url);
            return result ?? new List<BikeStation>();
        }

        // Versão simples (sem filtros) usada na página inicial
        public Task<List<BikeStation>> GetStationsAsync()
        {
            return GetStationsAsync(
                q: null,
                status: null,
                minBikes: null,
                sort: "name",
                dir: "asc",
                page: 1,
                pageSize: 50);
        }

        // GET one
        public Task<BikeStation?> GetStationAsync(int number)
        {
            return _http.GetFromJsonAsync<BikeStation>($"/api/v1/stations/{number}");
        }

        // POST
        public async Task<BikeStation?> CreateStationAsync(BikeStation station)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/stations", station);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Erro criando estação: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
            }

            return await response.Content.ReadFromJsonAsync<BikeStation>();
        }

        // PUT
        public async Task UpdateStationAsync(int number, BikeStation station)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/stations/{number}", station);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Erro atualizando estação: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
            }
        }

        // DELETE
        public async Task DeleteStationAsync(int number)
        {
            var response = await _http.DeleteAsync($"/api/v1/stations/{number}");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Erro apagando estação: {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");
            }
        }
    }
}
