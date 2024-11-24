using AuctionService.Data;
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.GraphQL;

public class Query
{
    public IQueryable<Auction> GetAuctions([Service] DataContext context)
    {
        return context.Auctions.Include(x => x.Item);
    }
}