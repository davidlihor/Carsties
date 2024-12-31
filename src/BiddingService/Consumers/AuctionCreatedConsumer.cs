using BiddingService.Data;
using BiddingService.Models;
using Contracts;
using MassTransit;
using MongoDB.Bson;

namespace BiddingService.Consumers;

public class AuctionCreatedConsumer(AppDbContext dbContext) : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        var auction = new Auction
        {
            Id = context.Message.Id,
            Seller = context.Message.Seller,
            AuctionEnd = context.Message.AuctionEnd,
            ReservePrice = context.Message.ReservePrice
        };

        await dbContext.Auctions.AddAsync(auction);
        await dbContext.SaveChangesAsync();
    }
}