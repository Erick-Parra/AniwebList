using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnimeList.Web.Auth;
using AnimeList.Web.Models;

namespace AnimeList.Web.Services;

public class ReviewApiService(HttpClient http, JwtAuthStateProvider authState)
{
    public async Task<ReviewResponse?> GetAsync(int malId)
    {
        var req = await BuildRequestAsync(HttpMethod.Get, $"api/reviews/{malId}");
        var res = await http.SendAsync(req);
        if (res.StatusCode == HttpStatusCode.NotFound) return null;
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<ReviewResponse>();
    }

    public async Task<ReviewResponse?> UpsertAsync(int malId, string content)
    {
        var req = await BuildRequestAsync(HttpMethod.Put, $"api/reviews/{malId}");
        req.Content = JsonContent.Create(new { content });
        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<ReviewResponse>();
    }

    public async Task<bool> DeleteAsync(int malId)
    {
        var req = await BuildRequestAsync(HttpMethod.Delete, $"api/reviews/{malId}");
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
