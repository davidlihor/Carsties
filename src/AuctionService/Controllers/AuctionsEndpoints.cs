using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Carter;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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
        DataContext context,
        IMapper mapper,
        HttpContext httpContext,
        IPublishEndpoint publishEndpoint)
    {
        var auction = mapper.Map<Auction>(request);
        auction.Seller = httpContext.User.Identity.Name;
        context.Auctions.Add(auction);
        
        var newAuction = mapper.Map<AuctionDto>(auction);
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));
        var result = await context.SaveChangesAsync() > 0;
        
        return result ? 
            Results.CreatedAtRoute(nameof(GetAuction), new { auction.Id }, newAuction) : 
            Results.BadRequest("Could not save changes to DB");
    }

    private static async Task<IResult> GetAuction(
        DataContext context, 
        IMapper mapper, 
        Guid id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        return auction == null ? Results.NotFound() : Results.Ok(mapper.Map<AuctionDto>(auction));
    }

    private static async Task<IResult> GetAuctions(string date,
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
        UpdateAuctionDto request,
        DataContext context,
        IMapper mapper,
        Guid id,
        HttpContext httpContext,
        IPublishEndpoint publishEndpoint)
    {
        var auction = await context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction is null) return Results.NotFound();
        if(auction.Seller != httpContext.User.Identity.Name) return Results.Forbid();
        
        auction.Item.Make = request.Make ?? auction.Item.Make;
        auction.Item.Model = request.Model ?? auction.Item.Model;
        auction.Item.Color= request.Color ?? auction.Item.Color;
        auction.Item.Mileage = request.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = request.Year ?? auction.Item.Year;
        
        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));
        var result = await context.SaveChangesAsync() > 0;
        
        return result ? Results.Ok() : Results.BadRequest("Could not save changes to DB");
    }

    private static async Task<IResult> DeleteAuction(
        DataContext context,
        HttpContext httpContext,
        Guid id,
        IPublishEndpoint publishEndpoint)
    {
        var auction = await context.Auctions.FindAsync(id);
        
        if(auction is null) return Results.NotFound();
        if(auction.Seller != httpContext.User.Identity.Name) return Results.Forbid();
        
        context.Auctions.Remove(auction);
        
        await publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id });
        var result = await context.SaveChangesAsync() > 0;
        
        return result ? Results.Ok() : Results.BadRequest("Could not save changes to DB");
    }
}