using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class BidPlacedConsumer(DataContext dbContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> Consume bid placed");
        var auction = await dbContext.Auctions.FindAsync(context.Message.AuctionId);
        if (auction is null) return;
        
        if (context.Message.BidStatus.Contains("Accepted") && context.Message.Amount > auction.CurrentHighBid || 
            auction.CurrentHighBid is null)
        {
            auction.CurrentHighBid = context.Message.Amount;
            await dbContext.SaveChangesAsync();
        }
    }
}