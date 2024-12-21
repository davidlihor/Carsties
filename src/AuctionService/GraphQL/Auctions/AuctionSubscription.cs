using AuctionService.Models;

namespace AuctionService.GraphQL.Auctions;

public class AuctionSubscription
{
    [Subscribe]
    [Topic]
    public Auction OnAuctionAdded([EventMessage] Auction auction) => auction;
}