namespace AnimeList.Core.DTOs.Stats;

public record StatsResponse(
    int TotalAnimes,
    int Watching,
    int Completed,
    int PlanToWatch,
    int OnHold,
    int Dropped,
    int TotalEpisodesWatched,
    double? AverageScore,
    int TotalFavorites,
    double HoursWatched);
