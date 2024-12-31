using AuctionService;
using BiddingService.Models;
using Grpc.Net.Client;

namespace BiddingService.Services;

public class GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
{
    public Auction GetAuction(Guid auctionId)
    {
        logger.LogInformation("Calling grpc service");
        var channel = GrpcChannel.ForAddress(config["GrpcAuction"]!);
        var client = new GrpcAuction.GrpcAuctionClient(channel);
        var request = new GetAuctionRequest{Id = auctionId.ToString()};

        try
        {
            var reply = client.GetAuction(request);
            var auction = new Auction
            {
                Id = Guid.Parse(reply.Auction.Id),
                AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
                Seller = reply.Auction.Seller,
                ReservePrice = reply.Auction.ReservedPrice
            };
            return auction;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not call gRPC Server");
            return null;
        }
    }
}