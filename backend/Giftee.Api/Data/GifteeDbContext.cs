using Giftee.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Giftee.Api.Data;

public class GifteeDbContext(DbContextOptions<GifteeDbContext> options) : DbContext(options)
{
    public DbSet<Gift> Gifts => Set<Gift>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Gift>(e =>
        {
            e.Property(g => g.Name).HasMaxLength(200).IsRequired();
            e.Property(g => g.Category).HasMaxLength(100).IsRequired();
            e.Property(g => g.MinPrice).HasPrecision(10, 2);
            e.Property(g => g.MaxPrice).HasPrecision(10, 2);
        });
    }
}