using AnimeList.Core.DTOs.Stats;

namespace AnimeList.Core.Interfaces;

public interface IStatsService
{
    Task<StatsResponse> GetStatsAsync(string userId, CancellationToken ct = default);
}
