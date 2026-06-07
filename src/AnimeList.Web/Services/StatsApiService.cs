using System.Net.Http.Headers;
using System.Net.Http.Json;
using AnimeList.Web.Auth;
using AnimeList.Web.Models;

namespace AnimeList.Web.Services;

public class StatsApiService(HttpClient http, JwtAuthStateProvider authState)
{
    public async Task<StatsResponse?> GetStatsAsync()
    {
        var token = await authState.GetTokenAsync();
        var req = new HttpRequestMessage(HttpMethod.Get, "api/stats");
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var res = await http.SendAsync(req);
        if (!res.IsSuccessStatusCode) return null;
        return await res.Content.ReadFromJsonAsync<StatsResponse>();
    }
}
