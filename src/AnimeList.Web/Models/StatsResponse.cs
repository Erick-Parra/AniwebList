namespace AnimeList.Web.Models;

public class StatsResponse
{
    public int TotalAnimes { get; set; }
    public int Watching { get; set; }
    public int Completed { get; set; }
    public int PlanToWatch { get; set; }
    public int OnHold { get; set; }
    public int Dropped { get; set; }
    public int TotalEpisodesWatched { get; set; }
    public double? AverageScore { get; set; }
    public int TotalFavorites { get; set; }
    public double HoursWatched { get; set; }
}
