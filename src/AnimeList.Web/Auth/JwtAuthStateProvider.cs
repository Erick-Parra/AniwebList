using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace AnimeList.Web.Auth;

public class JwtAuthStateProvider(IJSRuntime js) : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", "authToken");
        if (string.IsNullOrWhiteSpace(token)) return Anonymous;

        var claims = ParseClaims(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task NotifyLoginAsync(string token)
    {
        await js.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        var claims = ParseClaims(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var state = new AuthenticationState(new ClaimsPrincipal(identity));
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    public async Task NotifyLogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    public async Task<string?> GetTokenAsync() =>
        await js.InvokeAsync<string?>("localStorage.getItem", "authToken");

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var padded = (payload.Length % 4) switch { 2 => payload + "==", 3 => payload + "=", _ => payload };
        var bytes = Convert.FromBase64String(padded.Replace('-', '+').Replace('_', '/'));
        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(bytes) ?? [];
        return dict.Select(kv => new Claim(kv.Key, kv.Value.ToString()));
    }
}
