using AuctionService.Data;
using AuctionService.Models;

namespace AuctionService.GraphQL.Auctions;

public class AuctionType : ObjectType<Auction>
{
    protected override void Configure(IObjectTypeDescriptor<Auction> descriptor)
    {
        descriptor.Description("Represents Auction object");
        descriptor
            .Field(x => x.Product)
            .ResolveWith<Resolvers>(x => x.GetItem(default!, default!))
            .Description("Item which the auction belongs");
    }

    private class Resolvers
    {
        public IQueryable<Product> GetItem([Service] DataContext context, [Parent] Auction auction)
        {
            return context.Items.Where(x => x.AuctionId == auction.Id).AsQueryable();
        }
    }
}