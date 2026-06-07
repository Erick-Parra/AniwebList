using AnimeList.Core.DTOs.Stats;
using AnimeList.Core.Enums;
using AnimeList.Core.Interfaces;
using AnimeList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AnimeList.Infrastructure.Services;

public sealed class StatsService(AppDbContext db) : IStatsService
{
    public async Task<StatsResponse> GetStatsAsync(string userId, CancellationToken ct = default)
    {
        var entries = await db.UserAnimeEntries
            .Where(e => e.UserId == userId)
            .Select(e => new { e.WatchStatus, e.EpisodesWatched, e.Score, e.IsFavorite })
            .ToListAsync(ct);

        var scores = entries
            .Where(e => e.Score.HasValue)
            .Select(e => (double)e.Score!.Value)
            .ToList();

        var totalEpisodes = entries.Sum(e => e.EpisodesWatched);

        return new StatsResponse(
            TotalAnimes:          entries.Count,
            Watching:             entries.Count(e => e.WatchStatus == WatchStatus.Watching),
            Completed:            entries.Count(e => e.WatchStatus == WatchStatus.Completed),
            PlanToWatch:          entries.Count(e => e.WatchStatus == WatchStatus.PlanToWatch),
            OnHold:               entries.Count(e => e.WatchStatus == WatchStatus.OnHold),
            Dropped:              entries.Count(e => e.WatchStatus == WatchStatus.Dropped),
            TotalEpisodesWatched: totalEpisodes,
            AverageScore:         scores.Count > 0 ? Math.Round(scores.Average(), 1) : null,
            TotalFavorites:       entries.Count(e => e.IsFavorite),
            HoursWatched:         Math.Round(totalEpisodes * 24.0 / 60.0, 1));
    }
}
