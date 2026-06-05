using AnimeList.Core.DTOs.AnimeList;
using AnimeList.Core.Entities;
using AnimeList.Core.Enums;
using AnimeList.Core.Interfaces;
using AnimeList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AnimeList.Infrastructure.Services;

public sealed class AnimeListService(AppDbContext db, IJikanService jikanService) : IAnimeListService
{
    public async Task<IEnumerable<AnimeEntryResponse>> GetListAsync(
        string userId, WatchStatus? filter = null, CancellationToken ct = default)
    {
        var query = db.UserAnimeEntries
            .Include(e => e.Anime)
            .Where(e => e.UserId == userId);

        if (filter.HasValue)
            query = query.Where(e => e.WatchStatus == filter.Value);

        var entries = await query.OrderByDescending(e => e.UpdatedAt).ToListAsync(ct);
        return entries.Select(MapToResponse);
    }

    public async Task<AnimeEntryResponse?> GetEntryAsync(string userId, int entryId, CancellationToken ct = default)
    {
        var entry = await db.UserAnimeEntries
            .Include(e => e.Anime)
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId, ct);
        return entry is null ? null : MapToResponse(entry);
    }

    public async Task<AnimeEntryResponse?> AddAsync(string userId, int malId, CancellationToken ct = default)
    {
        var anime = await jikanService.ObtenerAnimePorMalIdAsync(malId, ct);
        if (anime is null) return null;

        var existing = await db.UserAnimeEntries
            .Include(e => e.Anime)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.AnimeId == anime.Id, ct);

        if (existing is not null) return MapToResponse(existing);

        var entry = new UserAnimeEntry
        {
            UserId = userId,
            AnimeId = anime.Id,
            WatchStatus = WatchStatus.PlanToWatch,
            EpisodesWatched = 0,
            IsFavorite = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.UserAnimeEntries.Add(entry);
        await db.SaveChangesAsync(ct);

        entry.Anime = anime;
        return MapToResponse(entry);
    }

    public async Task<AnimeEntryResponse?> UpdateAsync(
        string userId, int entryId, UpdateAnimeEntryRequest request, CancellationToken ct = default)
    {
        var entry = await db.UserAnimeEntries
            .Include(e => e.Anime)
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId, ct);

        if (entry is null) return null;

        entry.WatchStatus = request.WatchStatus;
        entry.EpisodesWatched = request.EpisodesWatched;
        entry.Score = request.Score;
        entry.IsFavorite = request.IsFavorite;
        entry.StartedAt = request.StartedAt;
        entry.FinishedAt = request.FinishedAt;
        entry.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return MapToResponse(entry);
    }

    public async Task<bool> RemoveAsync(string userId, int entryId, CancellationToken ct = default)
    {
        var entry = await db.UserAnimeEntries
            .FirstOrDefaultAsync(e => e.Id == entryId && e.UserId == userId, ct);

        if (entry is null) return false;

        db.UserAnimeEntries.Remove(entry);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static AnimeEntryResponse MapToResponse(UserAnimeEntry e) => new(
        e.Id,
        e.Anime.MalId,
        e.Anime.Title,
        e.Anime.TitleEnglish,
        e.Anime.ImageUrl,
        e.Anime.Synopsis,
        e.Anime.EpisodeCount,
        e.Anime.AirStatus,
        e.WatchStatus,
        e.EpisodesWatched,
        e.Score,
        e.IsFavorite,
        e.StartedAt,
        e.FinishedAt,
        e.CreatedAt,
        e.UpdatedAt);
}
