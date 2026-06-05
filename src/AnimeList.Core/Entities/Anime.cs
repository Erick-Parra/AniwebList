using AnimeList.Core.Enums;

namespace AnimeList.Core.Entities;

public class Anime
{
    public int Id { get; set; }
    public int MalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? TitleEnglish { get; set; }
    public string? ImageUrl { get; set; }
    public string? Synopsis { get; set; }
    public int? EpisodeCount { get; set; }
    public AnimeAirStatus AirStatus { get; set; }
    public string? Season { get; set; }
    public int? Year { get; set; }
    public DateTime CachedAt { get; set; }

    public ICollection<UserAnimeEntry> UserEntries { get; set; } = [];
}
