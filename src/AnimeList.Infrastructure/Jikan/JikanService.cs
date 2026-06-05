using System.Net.Http.Json;
using AnimeList.Core.Entities;
using AnimeList.Core.Enums;
using AnimeList.Core.Interfaces;
using AnimeList.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AnimeList.Infrastructure.Jikan;

public sealed class JikanService(
    IHttpClientFactory httpClientFactory,
    AppDbContext db,
    ILogger<JikanService> logger) : IJikanService
{
    // Días antes de refrescar el cache según el estado de emisión
    private const int CacheDaysAiring = 1;
    private const int CacheDaysDefault = 30;

    // Rate limiter: máximo 3 req/s → mínimo 350 ms entre llamadas
    private static readonly SemaphoreSlim RateLimiter = new(1, 1);
    private static DateTime _lastRequest = DateTime.MinValue;
    private const int MinIntervalMs = 350;

    public async Task<IEnumerable<Anime>> BuscarAnimesAsync(string query, CancellationToken ct = default)
    {
        var client = httpClientFactory.CreateClient("Jikan");

        JikanSearchResponse? response = null;
        try
        {
            await ThrottleAsync(ct);
            response = await client.GetFromJsonAsync<JikanSearchResponse>(
                $"anime?q={Uri.EscapeDataString(query)}&limit=20", ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error al buscar animes en Jikan con query '{Query}'", query);
            return [];
        }

        if (response?.Data is null) return [];

        var animes = new List<Anime>();
        foreach (var data in response.Data)
        {
            var anime = await SyncAnimeAsync(data, ct);
            animes.Add(anime);
        }

        return animes;
    }

    public async Task<Anime?> ObtenerAnimePorMalIdAsync(int malId, CancellationToken ct = default)
    {
        var cached = await db.Animes.FirstOrDefaultAsync(a => a.MalId == malId, ct);
        if (cached is not null && !NeedsRefresh(cached))
            return cached;

        JikanSingleResponse? response = null;
        try
        {
            var client = httpClientFactory.CreateClient("Jikan");
            await ThrottleAsync(ct);
            response = await client.GetFromJsonAsync<JikanSingleResponse>($"anime/{malId}", ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error al obtener anime MalId={MalId} de Jikan", malId);
            return cached; // devuelve cache viejo si falla la red
        }

        if (response?.Data is null) return cached;

        return await SyncAnimeAsync(response.Data, ct);
    }

    // Guarda o actualiza el anime en la BD y devuelve la entidad.
    private async Task<Anime> SyncAnimeAsync(JikanAnimeData data, CancellationToken ct)
    {
        var anime = await db.Animes.FirstOrDefaultAsync(a => a.MalId == data.MalId, ct);

        if (anime is null)
        {
            anime = new Anime { MalId = data.MalId };
            db.Animes.Add(anime);
        }

        anime.Title = data.Title;
        anime.TitleEnglish = string.IsNullOrWhiteSpace(data.TitleEnglish) ? null : data.TitleEnglish;
        anime.ImageUrl = data.Images?.Jpg?.ImageUrl;
        anime.Synopsis = string.IsNullOrWhiteSpace(data.Synopsis) ? null : data.Synopsis;
        anime.EpisodeCount = data.Episodes;
        anime.AirStatus = MapStatus(data.Status);
        anime.Season = data.Season;
        anime.Year = data.Year;
        anime.CachedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return anime;
    }

    private static bool NeedsRefresh(Anime anime)
    {
        var maxAge = anime.AirStatus == AnimeAirStatus.Airing ? CacheDaysAiring : CacheDaysDefault;
        return (DateTime.UtcNow - anime.CachedAt).TotalDays >= maxAge;
    }

    private static AnimeAirStatus MapStatus(string? status) => status switch
    {
        "Currently Airing" => AnimeAirStatus.Airing,
        "Not yet aired"    => AnimeAirStatus.NotYetAired,
        _                  => AnimeAirStatus.Finished,
    };

    private static async Task ThrottleAsync(CancellationToken ct)
    {
        await RateLimiter.WaitAsync(ct);
        try
        {
            var elapsed = (DateTime.UtcNow - _lastRequest).TotalMilliseconds;
            if (elapsed < MinIntervalMs)
                await Task.Delay((int)(MinIntervalMs - elapsed), ct);
            _lastRequest = DateTime.UtcNow;
        }
        finally
        {
            RateLimiter.Release();
        }
    }
}
