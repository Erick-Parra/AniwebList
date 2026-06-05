using AnimeList.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnimeList.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Anime> Animes => Set<Anime>();
    public DbSet<UserAnimeEntry> UserAnimeEntries => Set<UserAnimeEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Anime>(anime =>
        {
            anime.HasIndex(a => a.MalId).IsUnique();
        });

        builder.Entity<UserAnimeEntry>(entry =>
        {
            entry.HasIndex(e => new { e.UserId, e.AnimeId }).IsUnique();

            entry.HasOne(e => e.User)
                 .WithMany(u => u.AnimeEntries)
                 .HasForeignKey(e => e.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

            entry.HasOne(e => e.Anime)
                 .WithMany(a => a.UserEntries)
                 .HasForeignKey(e => e.AnimeId)
                 .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
