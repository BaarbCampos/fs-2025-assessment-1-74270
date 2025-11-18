using fs_2025_assessment_1_74270.Models;

namespace fs_2025_assessment_1_74270.Services
{
    public class BikeService
    {
        private readonly BikeRepository _repository;

        public BikeService(BikeRepository repository)
        {
            _repository = repository;
        }

        // Função para calcular a ocupação (0 a 1)
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
            bool desc,
            int page,
            int pageSize)
        {
            // 🔥 Importante: DEVE SER IEnumerable
            IEnumerable<BikeStation> query = _repository.GetAll();

            // ---------------------------
            // 1) FILTRO POR STATUS
            // ---------------------------
            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.ToUpper();
                query = query.Where(s => s.status.ToUpper() == status);
            }

            // ---------------------------
            // 2) FILTRO POR MÍNIMO DE BICICLETAS
            // ---------------------------
            if (minBikes.HasValue)
            {
                query = query.Where(s => s.available_bikes >= minBikes.Value);
            }

            // ---------------------------
            // 3) BUSCA (NAME + ADDRESS)
            // ---------------------------
            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.ToLower();
                query = query.Where(s =>
                    (s.name != null && s.name.ToLower().Contains(term)) ||
                    (s.address != null && s.address.ToLower().Contains(term))
                );
            }

            // ---------------------------
            // 4) SORTING (ORDENAÇÃO)
            // ---------------------------
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
                    // Ordenação padrão: por número
                    query = query.OrderBy(s => s.number);
                    break;
            }

            // ---------------------------
            // 5) PAGINAÇÃO
            // ---------------------------
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return query;
        }

        // Para pegar apenas 1 estação
        public BikeStation? GetByNumber(int number)
        {
            return _repository.Get(number);
        }
    }
}
