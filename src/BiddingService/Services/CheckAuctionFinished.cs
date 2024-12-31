using BiddingService.Data;
using BiddingService.Models;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BiddingService.Services;

public class CheckAuctionFinished(
    ILogger<CheckAuctionFinished> logger, 
    IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting check for finished auctions");
        
        stoppingToken.Register(() => logger.LogInformation("--> Stopping check for finished auctions"));

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAuctions(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task CheckAuctions(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        var finishedAuctions = await dbContext.Auctions
            .Where(x => x.AuctionEnd <= DateTime.UtcNow && !x.Finished)
            .ToListAsync(stoppingToken);

        if(finishedAuctions.Count == 0) return;
        logger.LogInformation("--> Found {count} finished auctions", finishedAuctions.Count);
        
        foreach (var auction in finishedAuctions)
        {
            auction.Finished = true;
            await dbContext.SaveChangesAsync(stoppingToken);
            
            var winningBid = await dbContext.Bids
                .Where(x => x.AuctionId == auction.Id && x.BidStatus == BidStatus.Accepted)
                .OrderByDescending(x => x.Amount)
                .FirstOrDefaultAsync(stoppingToken);

            await publishEndpoint.Publish(new AuctionFinished
            {
                ItemSold = winningBid is not null,
                AuctionId = auction.Id,
                Winner = winningBid?.Bidder,
                Amount = winningBid?.Amount,
                Seller = auction.Seller
            }, stoppingToken);
        }
    }
}