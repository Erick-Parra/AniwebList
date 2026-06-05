using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AnimeList.Infrastructure.Data;

// Solo usada por las herramientas de EF Core (dotnet ef migrations).
// En runtime, el DbContext lo configura el DI container con User Secrets.
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AniwebList_Dev;Trusted_Connection=True;")
            .Options;

        return new AppDbContext(options);
    }
}
