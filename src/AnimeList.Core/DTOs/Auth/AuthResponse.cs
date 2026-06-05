namespace AnimeList.Core.DTOs.Auth;

public record AuthResponse(string Token, DateTime ExpiresAt);
