namespace AnimeList.Web.Models;

public class ReviewResponse
{
    public int Id { get; set; }
    public int MalId { get; set; }
    public string AnimeTitle { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
