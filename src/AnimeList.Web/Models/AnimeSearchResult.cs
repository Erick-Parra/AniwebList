namespace AnimeList.Web.Models;

public record AnimeSearchResult(
    int MalId,
    string Title,
    string? TitleEnglish,
    string? ImageUrl,
    string? Synopsis,
    int? EpisodeCount,
    AnimeAirStatus AirStatus);
