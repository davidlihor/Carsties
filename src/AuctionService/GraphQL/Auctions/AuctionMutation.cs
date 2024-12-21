using System.Security.Claims;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Models;
using AutoMapper;
using Contracts;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.GraphQL.Auctions;

public class AuctionMutation
{
    [Authorize]
    public async Task<CreateAuctionPayload> CreateAuctionAsync(CreateAuctionInput input, ClaimsPrincipal user, 
        [Service] IMapper mapper,
        [Service] DataContext context, 
        [Service] IPublishEndpoint publishEndpoint,
        [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(string.IsNullOrEmpty(userId)) throw new GraphQLException("Unauthorized");
        
        var auction = mapper.Map<Auction>(input);
        auction.Seller = userId;
        
        context.Auctions.Add(auction);
        
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(auction), cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
    
        await eventSender.SendAsync(nameof(AuctionSubscription.OnAuctionAdded), auction, cancellationToken);
        
        return result ? new CreateAuctionPayload { Auction = mapper.Map<AuctionDto>(auction) } : throw new GraphQLException(
            ErrorBuilder.New().SetMessage("Database create failed").SetCode("DATABASE_ERROR").Build());
    }
    
    [Authorize]
    public async Task<UpdateAuctionPayload> UpdateAuctionAsync(UpdateAuctionInput input, ClaimsPrincipal user, 
        [Service] IMapper mapper,
        [Service] DataContext context, 
        [Service] IPublishEndpoint publishEndpoint, 
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == input.Id, cancellationToken);
    
        if (auction is null) throw new GraphQLException("Auction not found");
        if (auction.Seller != userId) throw new GraphQLException("Unauthorized");
        
        auction.Item.Make = input.Make ?? auction.Item.Make;
        auction.Item.Model = input.Model ?? auction.Item.Model;
        auction.Item.Color= input.Color ?? auction.Item.Color;
        auction.Item.Mileage = input.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = input.Year ?? auction.Item.Year;
        
        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction), cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
    
        return result ? new UpdateAuctionPayload { Auction = mapper.Map<AuctionDto>(auction) } : throw new GraphQLException(
            ErrorBuilder.New().SetMessage("Database update failed").SetCode("DATABASE_ERROR").Build());
    }
    
    [Authorize]
    public async Task<DeleteAuctionPayload> DeleteAuctionAsync(DeleteAuctionInput input, ClaimsPrincipal user, 
        [Service] IMapper mapper,
        [Service] DataContext context,
        [Service] IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var auction = await context.Auctions.FindAsync([input.Id], cancellationToken);
              
        if (auction is null) throw new GraphQLException("Auction not found");
        if (auction.Seller != userId) throw new GraphQLException("Unauthorized");
              
        context.Auctions.Remove(auction);
              
        await publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id }, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
    
        return result ? new DeleteAuctionPayload { AuctionId = input.Id } : throw new GraphQLException(
            ErrorBuilder.New().SetMessage("Database delete failed").SetCode("DATABASE_ERROR").Build());
    }
}