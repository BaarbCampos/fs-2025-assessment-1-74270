using Microsoft.AspNetCore.Mvc;
using fs_2025_assessment_1_74270.Models;
using fs_2025_assessment_1_74270.Services;

namespace fs_2025_assessment_1_74270.Controllers.V1
{
    [ApiController]
    [Route("api/v1/stations")]
    public class BikeController : ControllerBase
    {
        private readonly BikeRepository _repository;
        private readonly BikeQueryService _queryService;

        public BikeController(BikeRepository repository, BikeQueryService queryService)
        {
            _repository = repository;
            _queryService = queryService;
        }

        // ====== READ ALL com filtros/busca/sort/paging ======
        // GET: /api/v1/stations
        [HttpGet]
        public ActionResult<IEnumerable<BikeStation>> GetAll(
            [FromQuery] string? status,
            [FromQuery] int? minBikes,
            [FromQuery] string? q,
            [FromQuery] string? sort,
            [FromQuery] string? dir,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = _queryService.GetStations(
                status, minBikes, q, sort, dir, page, pageSize);

            return Ok(result);
        }

        // ====== SUMMARY ======
        // GET: /api/v1/stations/summary
        [HttpGet("summary")]
        public ActionResult<StationSummary> GetSummary()
        {
            var summary = _queryService.GetSummary();
            return Ok(summary);
        }

        // ====== READ ONE ======
        // GET: /api/v1/stations/{number}
        [HttpGet("{number:int}")]
        public ActionResult<BikeStation> GetByNumber(int number)
        {
            var station = _repository.Get(number);

            if (station == null)
                return NotFound();

            return Ok(station);
        }

        // ====== CREATE ======
        // POST: /api/v1/stations
        [HttpPost]
        public ActionResult<BikeStation> Create([FromBody] BikeStation station)
        {
            if (station == null)
                return BadRequest();

            var created = _repository.Add(station);

            return CreatedAtAction(nameof(GetByNumber),
                new { number = created.number }, created);
        }

        // ====== UPDATE ======
        // PUT: /api/v1/stations/{number}
        [HttpPut("{number:int}")]
        public IActionResult Update(int number, [FromBody] BikeStation station)
        {
            if (station == null)
                return BadRequest();

            var ok = _repository.Update(number, station);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // ====== DELETE ======
        // DELETE: /api/v1/stations/{number}
        [HttpDelete("{number:int}")]
        public IActionResult Delete(int number)
        {
            var ok = _repository.Delete(number);

            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}
