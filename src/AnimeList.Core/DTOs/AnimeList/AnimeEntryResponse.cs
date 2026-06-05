using AnimeList.Core.Enums;

namespace AnimeList.Core.DTOs.AnimeList;

public record AnimeEntryResponse(
    int Id,
    int MalId,
    string Title,
    string? TitleEnglish,
    string? ImageUrl,
    string? Synopsis,
    int? EpisodeCount,
    AnimeAirStatus AirStatus,
    WatchStatus WatchStatus,
    int EpisodesWatched,
    int? Score,
    bool IsFavorite,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);
