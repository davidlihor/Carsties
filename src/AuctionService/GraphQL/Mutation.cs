using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.GraphQL.Auctions;
using AutoMapper;

namespace AuctionService.GraphQL;

public class Mutation(IMapper mapper)
{
    public async Task<AuctionPayload> AddAuctionAsync(CreateAuctionDto input, [Service] DataContext context)
    {
        var auction = mapper.Map<Auction>(input);
        context.Auctions.Add(auction);
        await context.SaveChangesAsync();
        
        return mapper.Map<AuctionPayload>(auction);
    }
}