namespace AnimeList.Web.Models;

public class AnimeEntryResponse
{
    public int Id { get; set; }
    public int MalId { get; set; }
    public string Title { get; set; } = "";
    public string? TitleEnglish { get; set; }
    public string? ImageUrl { get; set; }
    public string? Synopsis { get; set; }
    public int? EpisodeCount { get; set; }
    public AnimeAirStatus AirStatus { get; set; }
    public WatchStatus WatchStatus { get; set; }
    public int EpisodesWatched { get; set; }
    public int? Score { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
