namespace AnimeList.Web.Models;

public class UpdateAnimeEntryRequest
{
    public WatchStatus WatchStatus { get; set; }
    public int EpisodesWatched { get; set; }
    public int? Score { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
