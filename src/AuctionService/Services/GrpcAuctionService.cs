using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services;

public class GrpcAuctionService(DataContext dbContext) : GrpcAuction.GrpcAuctionBase
{
    public override async Task<GetAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
    {
        Console.WriteLine("--> Received grpc request for auction");
        var auction = await dbContext.Auctions.FindAsync(Guid.Parse(request.Id));
        if (auction is null) throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"));

        var response = new GetAuctionResponse
        {
            Auction = new GrpcAuctionModel
            {
                Id = auction.Id.ToString(),
                AuctionEnd = auction.AuctionEnd.ToString(),
                ReservedPrice = auction.ReservePrice,
                Seller = auction.Seller,
            }
        };
        return response;
    }
}