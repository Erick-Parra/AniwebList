using AnimeList.Core.DTOs.Reviews;

namespace AnimeList.Core.Interfaces;

public interface IReviewService
{
    Task<ReviewResponse?> GetByMalIdAsync(string userId, int malId, CancellationToken ct = default);
    Task<ReviewResponse?> UpsertAsync(string userId, int malId, string content, CancellationToken ct = default);
    Task<bool> DeleteAsync(string userId, int malId, CancellationToken ct = default);
}
