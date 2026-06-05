using AnimeList.Core.DTOs.AnimeList;
using AnimeList.Core.Enums;

namespace AnimeList.Core.Interfaces;

public interface IAnimeListService
{
    Task<IEnumerable<AnimeEntryResponse>> GetListAsync(string userId, WatchStatus? filter = null, CancellationToken ct = default);
    Task<AnimeEntryResponse?> AddAsync(string userId, int malId, CancellationToken ct = default);
    Task<AnimeEntryResponse?> UpdateAsync(string userId, int entryId, UpdateAnimeEntryRequest request, CancellationToken ct = default);
    Task<bool> RemoveAsync(string userId, int entryId, CancellationToken ct = default);
}
