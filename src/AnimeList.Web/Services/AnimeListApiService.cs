using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AnimeList.Web.Auth;
using AnimeList.Web.Models;

namespace AnimeList.Web.Services;

public class AnimeListApiService(HttpClient http, JwtAuthStateProvider authState)
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<List<AnimeEntryResponse>> GetListAsync(WatchStatus? filter = null)
    {
        var url = filter.HasValue ? $"api/anime-list?status={filter}" : "api/anime-list";
        var req = await BuildRequestAsync(HttpMethod.Get, url);
        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode) return [];
        return await res.Content.ReadFromJsonAsync<List<AnimeEntryResponse>>(JsonOpts) ?? [];
    }

    public async Task<AnimeEntryResponse?> AddAsync(int malId)
    {
        var req = await BuildRequestAsync(HttpMethod.Post, "api/anime-list");
        req.Content = JsonContent.Create(new { malId });
        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<AnimeEntryResponse>(JsonOpts);
    }

    public async Task<bool> RemoveAsync(int entryId)
    {
        var req = await BuildRequestAsync(HttpMethod.Delete, $"api/anime-list/{entryId}");
        var res = await http.SendAsync(req);
        return res.IsSuccessStatusCode;
    }

    private async Task<HttpRequestMessage> BuildRequestAsync(HttpMethod method, string url)
    {
        var req = new HttpRequestMessage(method, url);
        var token = await authState.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return req;
    }
}
