using fs_2025_assessment_1_74270.Models;

namespace fs_2025_assessment_1_74270.Services
{
    public class StationSummary
    {
        public int TotalStations { get; set; }
        public int TotalBikeStands { get; set; }
        public int TotalAvailableBikes { get; set; }
        public int OpenStations { get; set; }
        public int ClosedStations { get; set; }
    }

    public class BikeQueryService
    {
        private readonly BikeRepository _repository;

        public BikeQueryService(BikeRepository repository)
        {
            _repository = repository;
        }

        private double Occupancy(BikeStation s)
        {
            if (s.bike_stands == 0) return 0;
            return (double)s.available_bikes / s.bike_stands;
        }

        public IEnumerable<BikeStation> GetStations(
            string? status,
            int? minBikes,
            string? search,
            string? sort,
            string? dir,
            int page,
            int pageSize)
        {
            IEnumerable<BikeStation> query = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(s =>
                    s.status != null &&
                    s.status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (minBikes.HasValue)
            {
                query = query.Where(s => s.available_bikes >= minBikes.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.ToLower();
                query = query.Where(s =>
                    (s.name != null && s.name.ToLower().Contains(term)) ||
                    (s.address != null && s.address.ToLower().Contains(term)));
            }

            bool desc = string.Equals(dir, "desc", StringComparison.OrdinalIgnoreCase);

            switch (sort?.ToLower())
            {
                case "name":
                    query = desc
                        ? query.OrderByDescending(s => s.name)
                        : query.OrderBy(s => s.name);
                    break;

                case "availablebikes":
                    query = desc
                        ? query.OrderByDescending(s => s.available_bikes)
                        : query.OrderBy(s => s.available_bikes);
                    break;

                case "occupancy":
                    query = desc
                        ? query.OrderByDescending(Occupancy)
                        : query.OrderBy(Occupancy);
                    break;

                default:
                    query = query.OrderBy(s => s.number);
                    break;
            }

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return query.ToList();
        }

        public StationSummary GetSummary()
        {
            var stations = _repository.GetAll();

            return new StationSummary
            {
                TotalStations = stations.Count,
                TotalBikeStands = stations.Sum(s => s.bike_stands),
                TotalAvailableBikes = stations.Sum(s => s.available_bikes),
                OpenStations = stations.Count(s =>
                    s.status != null &&
                    s.status.Equals("OPEN", StringComparison.OrdinalIgnoreCase)),
                ClosedStations = stations.Count(s =>
                    s.status != null &&
                    s.status.Equals("CLOSED", StringComparison.OrdinalIgnoreCase))
            };
        }
    }
}
