using AnimeList.Core.Enums;

namespace AnimeList.Core.DTOs.Anime;

public record AnimeSearchResult(
    int MalId,
    string Title,
    string? TitleEnglish,
    string? ImageUrl,
    string? Synopsis,
    int? EpisodeCount,
    AnimeAirStatus AirStatus);
