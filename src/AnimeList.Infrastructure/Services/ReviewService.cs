using AnimeList.Core.DTOs.Reviews;
using AnimeList.Core.Entities;
using AnimeList.Core.Interfaces;
using AnimeList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AnimeList.Infrastructure.Services;

public sealed class ReviewService(AppDbContext db) : IReviewService
{
    public async Task<ReviewResponse?> GetByMalIdAsync(string userId, int malId, CancellationToken ct = default)
    {
        var review = await db.Reviews
            .Include(r => r.Anime)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Anime.MalId == malId, ct);

        return review is null ? null : MapToResponse(review);
    }

    public async Task<ReviewResponse?> UpsertAsync(string userId, int malId, string content, CancellationToken ct = default)
    {
        var anime = await db.Animes.FirstOrDefaultAsync(a => a.MalId == malId, ct);
        if (anime is null) return null;

        var review = await db.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.AnimeId == anime.Id, ct);

        if (review is null)
        {
            review = new Review
            {
                UserId = userId,
                AnimeId = anime.Id,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            db.Reviews.Add(review);
        }
        else
        {
            review.Content = content;
            review.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        review.Anime = anime;
        return MapToResponse(review);
    }

    public async Task<bool> DeleteAsync(string userId, int malId, CancellationToken ct = default)
    {
        var review = await db.Reviews
            .Include(r => r.Anime)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.Anime.MalId == malId, ct);

        if (review is null) return false;

        db.Reviews.Remove(review);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static ReviewResponse MapToResponse(Review r) => new(
        r.Id,
        r.Anime.MalId,
        r.Anime.Title,
        r.Content,
        r.CreatedAt,
        r.UpdatedAt);
}
