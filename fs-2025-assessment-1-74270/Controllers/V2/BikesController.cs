using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using fs_2025_assessment_1_74270.Data;
using fs_2025_assessment_1_74270.Models;

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

        // ================================================================
        // GET: /api/v2/stations
        // ================================================================
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

            IEnumerable<BikeStation> query = stations;

            // ----- filtro status -----
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.ToUpperInvariant();
                query = query.Where(x => x.status != null &&
                                         x.status.ToUpper() == s);
            }

            // ----- filtro min bikes -----
            if (minBikes.HasValue)
            {
                query = query.Where(x => x.available_bikes >= minBikes.Value);
            }

            // ----- texto (nome / address) -----
            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.ToLower();
                query = query.Where(x =>
                    (x.name != null && x.name.ToLower().Contains(term)) ||
                    (x.address != null && x.address.ToLower().Contains(term)));
            }

            // ----- ordenação -----
            bool desc = string.Equals(dir, "desc", System.StringComparison.OrdinalIgnoreCase);

            switch (sort?.ToLower())
            {
                case "name":
                    query = desc ? query.OrderByDescending(x => x.name)
                                 : query.OrderBy(x => x.name);
                    break;

                case "availablebikes":
                    query = desc ? query.OrderByDescending(x => x.available_bikes)
                                 : query.OrderBy(x => x.available_bikes);
                    break;

                case "occupancy":
                    query = desc ? query.OrderByDescending(Occupancy)
                                 : query.OrderBy(Occupancy);
                    break;

                default:
                    query = query.OrderBy(x => x.number);
                    break;
            }

            // ----- paginação -----
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var paged = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

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

        // ================================================================
        // GET: /api/v2/stations/{number}
        // ================================================================
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

        // ================================================================
        // POST: /api/v2/stations
        // ================================================================
        [HttpPost]
        public async Task<ActionResult<BikeStation>> Create([FromBody] BikeStation station)
        {
            if (station == null)
                return BadRequest();

            var created = await _cosmos.AddAsync(station);

            return CreatedAtAction(nameof(GetByNumber),
                new { number = created.number }, created);
        }

        // ================================================================
        // PUT: /api/v2/stations/{number}
        // ================================================================
        [HttpPut("{number:int}")]
        public async Task<IActionResult> Update(int number, [FromBody] BikeStation station)
        {
            if (station == null)
                return BadRequest();

            var ok = await _cosmos.UpdateAsync(number, station);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // ================================================================
        // DELETE: /api/v2/stations/{number}
        // ================================================================
        [HttpDelete("{number:int}")]
        public async Task<IActionResult> Delete(int number)
        {
            var ok = await _cosmos.DeleteAsync(number);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // Função auxiliar: ocupação
        private static double Occupancy(BikeStation s)
        {
            if (s.bike_stands <= 0) return 0.0;
            return (double)s.available_bikes / s.bike_stands;
        }
    }
}
