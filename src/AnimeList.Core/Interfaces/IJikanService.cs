using AnimeList.Core.Entities;

namespace AnimeList.Core.Interfaces;

public interface IJikanService
{
    Task<IEnumerable<Anime>> BuscarAnimesAsync(string query, CancellationToken ct = default);
    Task<Anime?> ObtenerAnimePorMalIdAsync(int malId, CancellationToken ct = default);
}
