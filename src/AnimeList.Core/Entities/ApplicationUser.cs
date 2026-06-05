using Microsoft.AspNetCore.Identity;

namespace AnimeList.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserAnimeEntry> AnimeEntries { get; set; } = [];
}
