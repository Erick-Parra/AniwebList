namespace AnimeList.Core.DTOs.Auth;

public record RegisterRequest(string UserName, string Email, string Password);
