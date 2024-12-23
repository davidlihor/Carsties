using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Carter;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace AuctionService.Controllers;

public class AuctionsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    { 
        var group = app.MapGroup("api/auctions");

        group.MapGet("", GetAuctions).WithName("GetAuctions");
        group.MapGet("{id:guid}", GetAuction).WithName("GetAuction");
        group.MapPost("", CreateAuction).RequireAuthorization();
        group.MapPut("{id:guid}", UpdateAuction).RequireAuthorization();
        group.MapDelete("{id:guid}", DeleteAuction).RequireAuthorization();
    }

    private static async Task<IResult> CreateAuction(
        CreateAuctionDto request,
        HybridCache cache,
        DataContext context,
        IMapper mapper,
        HttpContext httpContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        var auction = mapper.Map<Auction>(request);
        auction.Seller = httpContext.User.Identity.Name;
        context.Auctions.Add(auction);
        
        var newAuction = mapper.Map<AuctionDto>(auction);
        
        await cache.SetAsync($"auctions-{auction.Id}", newAuction, 
            tags: ["auctions"], 
            cancellationToken: cancellationToken);
        
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(auction), cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        
        return result ? 
            Results.CreatedAtRoute(nameof(GetAuction), new { auction.Id }, newAuction) : 
            Results.BadRequest("Could not save changes to DB");
    }

    private static async Task<IResult> GetAuction(
        HybridCache cache,
        DataContext context, 
        IMapper mapper, Guid id,
        CancellationToken cancellationToken)
    {
        var cachedAuction = await cache.GetOrCreateAsync($"auctions-{id}", async factory =>
        { 
            var auction = await context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
            return mapper.Map<AuctionDto>(auction);
        },
        tags: ["auctions"],
        cancellationToken: cancellationToken);
        
        return cachedAuction is null ? Results.NotFound() : Results.Ok(cachedAuction);
    }

    private static async Task<IResult> GetAuctions(
        string date,
        DataContext context,
        IMapper mapper)
    {
        var query = context.Auctions
            .OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrWhiteSpace(date))
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);

        return Results.Ok(await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync());
    }

    private static async Task<IResult> UpdateAuction(
        HybridCache cache, Guid id,
        UpdateAuctionDto request,
        DataContext context,
        IMapper mapper,
        HttpContext httpContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        var auction = await context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        if(auction is null) return Results.NotFound();
        if(auction.Seller != httpContext.User.Identity.Name) return Results.Forbid();
        
        auction.Item.Make = request.Make ?? auction.Item.Make;
        auction.Item.Model = request.Model ?? auction.Item.Model;
        auction.Item.Color= request.Color ?? auction.Item.Color;
        auction.Item.Mileage = request.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = request.Year ?? auction.Item.Year;
        
        await cache.SetAsync($"auctions-{auction.Id}",
            mapper.Map<AuctionDto>(auction), 
            tags: ["auctions"], 
            cancellationToken: cancellationToken);
        
        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction), cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        
        return result ? Results.NoContent() : Results.BadRequest("Could not save changes to DB");
    }

    private static async Task<IResult> DeleteAuction(
        DataContext context, Guid id,
        HttpContext httpContext,
        HybridCache cache,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        var auction = await context.Auctions.FindAsync([id], cancellationToken);
        
        if(auction is null) return Results.NotFound();
        if(auction.Seller != httpContext.User.Identity.Name) return Results.Forbid();
        
        context.Auctions.Remove(auction);
        await cache.RemoveAsync($"auctions-{id}", cancellationToken);
        
        await publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id }, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken) > 0;
        
        return result ? Results.NoContent() : Results.BadRequest("Could not save changes to DB");
    }
}