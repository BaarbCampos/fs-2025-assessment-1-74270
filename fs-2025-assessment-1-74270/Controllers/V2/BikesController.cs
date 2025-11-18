using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using fs_2025_assessment_1_74270.Models;
using fs_2025_assessment_1_74270.Data;

namespace fs_2025_assessment_1_74270.Controllers.V2
{
    [ApiController]
    [Route("api/v2/stations")]
    public class BikesController : ControllerBase
    {
        private readonly CosmosBikeRepository _cosmos;

        public BikesController(CosmosBikeRepository cosmos)
        {
            _cosmos = cosmos;
        }

        // GET: /api/v2/stations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll(
            [FromQuery] string? status,
            [FromQuery] int? minBikes,
            [FromQuery] string? q,
            [FromQuery] string? sort,
            [FromQuery] string? dir,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var stations = await _cosmos.GetAllAsync();

            // Trabalhamos sempre com IEnumerable (LINQ em memória)
            IEnumerable<BikeStation> query = stations;

            // -------- filtros --------

            // status (OPEN/CLOSED)
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.ToUpperInvariant();
                query = query.Where(x => x.status != null &&
                                         x.status.ToUpper() == s);
            }

            // minBikes
            if (minBikes.HasValue)
            {
                query = query.Where(x => x.available_bikes >= minBikes.Value);
            }

            // busca por nome / endereço
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(x =>
                    (x.name != null && x.name.ToLower().Contains(term)) ||
                    (x.address != null && x.address.ToLower().Contains(term)));
            }

            // -------- ordenação --------
            bool desc = string.Equals(dir, "desc", System.StringComparison.OrdinalIgnoreCase);

            switch (sort?.ToLower())
            {
                case "name":
                    query = desc
                        ? query.OrderByDescending(x => x.name)
                        : query.OrderBy(x => x.name);
                    break;

                case "availablebikes":
                    query = desc
                        ? query.OrderByDescending(x => x.available_bikes)
                        : query.OrderBy(x => x.available_bikes);
                    break;

                case "occupancy":
                    query = desc
                        ? query.OrderByDescending(Occupancy)
                        : query.OrderBy(Occupancy);
                    break;

                default:
                    // padrão: ordena por número
                    query = query.OrderBy(x => x.number);
                    break;
            }

            // -------- paginação --------
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var paged = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            // Projeção simples (pode ser DTO depois)
            var result = paged.Select(station => new
            {
                station.number,
                station.name,
                station.address,
                station.position,
                station.available_bikes,
                station.available_bike_stands,
                station.status
            });

            return Ok(result);
        }

        // GET: /api/v2/stations/{number}
        [HttpGet("{number:int}")]
        public async Task<ActionResult<object>> GetByNumber(int number)
        {
            var station = await _cosmos.GetByNumberAsync(number);

            if (station == null)
                return NotFound();

            var result = new
            {
                station.number,
                station.name,
                station.address,
                station.position,
                station.available_bikes,
                station.available_bike_stands,
                station.status
            };

            return Ok(result);
        }

        // Função auxiliar: ocupação
        private static double Occupancy(BikeStation s)
        {
            if (s.bike_stands <= 0) return 0.0;
            return (double)s.available_bikes / s.bike_stands;
        }
    }
}
