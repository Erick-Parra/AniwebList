using System.Text.Json.Serialization;

namespace AnimeList.Infrastructure.Jikan;

internal sealed class JikanSearchResponse
{
    [JsonPropertyName("data")]
    public List<JikanAnimeData> Data { get; set; } = [];
}

internal sealed class JikanSingleResponse
{
    [JsonPropertyName("data")]
    public JikanAnimeData? Data { get; set; }
}

internal sealed class JikanAnimeData
{
    [JsonPropertyName("mal_id")]
    public int MalId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("title_english")]
    public string? TitleEnglish { get; set; }

    [JsonPropertyName("images")]
    public JikanImages? Images { get; set; }

    [JsonPropertyName("synopsis")]
    public string? Synopsis { get; set; }

    [JsonPropertyName("episodes")]
    public int? Episodes { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("season")]
    public string? Season { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }
}

internal sealed class JikanImages
{
    [JsonPropertyName("jpg")]
    public JikanImageSet? Jpg { get; set; }
}

internal sealed class JikanImageSet
{
    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}
