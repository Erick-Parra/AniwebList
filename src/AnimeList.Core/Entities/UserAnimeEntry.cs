using AnimeList.Core.Enums;

namespace AnimeList.Core.Entities;

public class UserAnimeEntry
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int AnimeId { get; set; }
    public WatchStatus WatchStatus { get; set; }
    public int EpisodesWatched { get; set; }
    public int? Score { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Anime Anime { get; set; } = null!;
}
