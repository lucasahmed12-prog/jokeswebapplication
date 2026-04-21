using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using jokesWebApp.Models;

namespace jokesWebApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Joke> Joke { get; set; } = default!;
    public DbSet<JokeLike> JokeLike { get; set; } = default!;
    public DbSet<JokeComment> JokeComment { get; set; } = default!;
}
