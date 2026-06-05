using System.Net.Http.Json;
using System.Text.Json;
using AnimeList.Web.Auth;
using AnimeList.Web.Models;

namespace AnimeList.Web.Services;

public class AuthService(HttpClient http, JwtAuthStateProvider authState)
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    public async Task<string?> LoginAsync(string email, string password)
    {
        var res = await http.PostAsJsonAsync("api/auth/login", new { email, password });
        if (!res.IsSuccessStatusCode) return "Credenciales incorrectas.";

        var auth = await res.Content.ReadFromJsonAsync<AuthResponse>(JsonOpts);
        if (auth is null) return "Error inesperado.";

        await authState.NotifyLoginAsync(auth.Token);
        return null;
    }

    public async Task<string?> RegisterAsync(string userName, string email, string password)
    {
        var res = await http.PostAsJsonAsync("api/auth/register", new { userName, email, password });
        if (!res.IsSuccessStatusCode)
        {
            var errors = await res.Content.ReadFromJsonAsync<string[]>(JsonOpts);
            return errors is { Length: > 0 } ? errors[0] : "Error al registrar.";
        }

        var auth = await res.Content.ReadFromJsonAsync<AuthResponse>(JsonOpts);
        if (auth is null) return "Error inesperado.";

        await authState.NotifyLoginAsync(auth.Token);
        return null;
    }

    public Task LogoutAsync() => authState.NotifyLogoutAsync();
}
