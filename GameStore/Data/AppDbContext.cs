using Microsoft.EntityFrameworkCore;
using GameStore.Models;

namespace GameStore.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<UserItem> UserItems { get; set; }
    public DbSet<MarketListing> MarketListings { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}