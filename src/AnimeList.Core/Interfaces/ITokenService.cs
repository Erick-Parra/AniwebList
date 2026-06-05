using AnimeList.Core.Entities;

namespace AnimeList.Core.Interfaces;

public interface ITokenService
{
    string GenerateToken(ApplicationUser user);
    DateTime GetExpiry();
}
