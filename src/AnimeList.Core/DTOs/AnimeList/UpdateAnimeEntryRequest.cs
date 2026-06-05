using AnimeList.Core.Enums;

namespace AnimeList.Core.DTOs.AnimeList;

public record UpdateAnimeEntryRequest(
    WatchStatus WatchStatus,
    int EpisodesWatched,
    int? Score,
    bool IsFavorite,
    DateTime? StartedAt,
    DateTime? FinishedAt);
