using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Carter;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

public class AuctionsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    { 
        var group = app.MapGroup("api/auctions");

        group.MapGet("", GetAuctions);
        group.MapGet("{id:guid}", GetAuction);
        group.MapPost("", CreateAuction);
        group.MapPut("{id:guid}", UpdateAuction);
        group.MapDelete("{id:guid}", DeleteAuction);
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

    private static async Task<IResult> GetAuctions(
        DataContext context,
        IMapper mapper)
    {
        var auctions = await context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();
        
        return Results.Ok(mapper.Map<List<AuctionDto>>(auctions));
    }

    private static async Task<IResult> CreateAuction(
        CreateAuctionDto request, 
        DataContext context,
        IMapper mapper)
    {
        var auction = mapper.Map<Auction>(request);
        // userId
        auction.Seller = "test";
        context.Auctions.Add(auction);
        var result = await context.SaveChangesAsync() > 0;
        
        return result ? 
            Results.CreatedAtRoute(nameof(GetAuction), new {id = auction.Id}, mapper.Map<AuctionDto>(auction)) :
            Results.BadRequest("Could not save changes to the DB");
    }

    private static async Task<IResult> UpdateAuction(
        UpdateAuctionDto request,
        DataContext context,
        IMapper mapper, Guid id)
    {
        var auction = await context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if(auction is null) return Results.NotFound();
        
        //userId
        auction.Item.Make = request.Make ?? auction.Item.Make;
        auction.Item.Model = request.Model ?? auction.Item.Model;
        auction.Item.Color= request.Color ?? auction.Item.Color;
        auction.Item.Mileage = request.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = request.Year ?? auction.Item.Year;
        //auction = mapper.Map<Auction>(request);
        
        var result = await context.SaveChangesAsync() > 0;
        
        return result ? Results.Ok() : Results.BadRequest("Could not save changes to the DB");
    }

    private static async Task<IResult> DeleteAuction(
        DataContext context, Guid id)
    {
        var auction = await context.Auctions.FindAsync(id);
        
        if(auction is null) return Results.NotFound();
        
        //userId
        context.Auctions.Remove(auction);
        var result = await context.SaveChangesAsync() > 0;
        
        return result ? Results.Ok() : Results.BadRequest("Could not save changes to the DB");
    }
}