using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AnimeList.Core.Entities;
using AnimeList.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AnimeList.Infrastructure.Auth;

public sealed class TokenService(IConfiguration config) : ITokenService
{
    private const int DefaultExpiryDays = 7;

    public string GenerateToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: GetExpiry(),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpiry()
    {
        var days = int.TryParse(config["Jwt:ExpiryDays"], out var d) ? d : DefaultExpiryDays;
        return DateTime.UtcNow.AddDays(days);
    }
}
