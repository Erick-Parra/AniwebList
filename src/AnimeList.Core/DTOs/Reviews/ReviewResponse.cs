namespace AnimeList.Core.DTOs.Reviews;

public record ReviewResponse(
    int Id,
    int MalId,
    string AnimeTitle,
    string Content,
    DateTime CreatedAt,
    DateTime UpdatedAt);
