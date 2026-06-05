using AnimeList.Core.Interfaces;
using AnimeList.Infrastructure.Jikan;
using Microsoft.Extensions.DependencyInjection;

namespace AnimeList.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddHttpClient("Jikan", client =>
        {
            client.BaseAddress = new Uri("https://api.jikan.moe/v4/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IJikanService, JikanService>();

        return services;
    }
}
