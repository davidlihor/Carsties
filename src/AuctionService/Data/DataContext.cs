using AuctionService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Auction> Auctions { get; set; }
    public DbSet<Product> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Auction>()
            .HasOne(a => a.Product)
            .WithOne(i => i.Auction)
            .HasForeignKey<Product>(i => i.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}