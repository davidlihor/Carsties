using System.ComponentModel.DataAnnotations;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Models;
using AutoMapper;
using Carter;
using Contracts;
using MassTransit;
using Microsoft.Extensions.Caching.Hybrid;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

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

    public static async Task<IResult> CreateAuction(
        CreateAuctionDto request,
        //HybridCache cache,
        IAuctionRepository repository,
        IMapper mapper,
        HttpContext httpContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken ct)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            return Results.BadRequest(validationResults);
        }
        
        var auction = mapper.Map<Auction>(request);
        auction.Seller = httpContext.User.Identity.Name;
        repository.CreateAuction(auction);
        
        //await cache.SetAsync($"auctions-{auction.Id}", newAuction, null, ["auctions"], ct);
        
        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(auction), ct);
        var result = await repository.SaveChangesAsync();
        
        return result ? 
            Results.CreatedAtRoute(nameof(GetAuction), new { auction.Id }, mapper.Map<AuctionDto>(auction)) : 
            Results.BadRequest("Could not save changes to DB");
    }

    public static async Task<IResult> GetAuction(
      //  HybridCache cache,
        IAuctionRepository repository,
        Guid id,
        CancellationToken cancellationToken)
    {
        //var cachedAuction = await cache.GetOrCreateAsync($"auctions-{id}", async factory =>
        //{         
        var auction = await repository.GetAuctionByIdAsync(id);
        //}
        // tags: ["auctions"],
        // cancellationToken: cancellationToken);
        
        return auction is null ? Results.NotFound() : Results.Ok(auction);
    }

    public static async Task<IResult> GetAuctions(
        string date,
        IAuctionRepository repository)
    {
        return Results.Ok(await repository.GetAuctionsAsync(date));
    }

    public static async Task<IResult> UpdateAuction(
        Guid id,
        //HybridCache cache,
        UpdateAuctionDto request,
        IAuctionRepository repository,
        IMapper mapper,
        HttpContext httpContext,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        var auction = await repository.GetAuctionModelById(id);
        
        if(auction is null) return Results.NotFound();
        if(auction.Seller != httpContext.User.Identity.Name) return Results.Forbid();
        
        auction.Product.Make = request.Make ?? auction.Product.Make;
        auction.Product.Model = request.Model ?? auction.Product.Model;
        auction.Product.Color= request.Color ?? auction.Product.Color;
        auction.Product.Mileage = request.Mileage ?? auction.Product.Mileage;
        auction.Product.Year = request.Year ?? auction.Product.Year;
        
        // await cache.SetAsync($"auctions-{auction.Id}",
        //     mapper.Map<AuctionDto>(auction), 
        //     tags: ["auctions"], 
        //     cancellationToken: cancellationToken);
        
        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction), cancellationToken);
        var result = await repository.SaveChangesAsync();
        
        return result ? Results.NoContent() : Results.BadRequest("Could not save changes to DB");
    }

    public static async Task<IResult> DeleteAuction(
        Guid id,
        IAuctionRepository repository,
        HttpContext httpContext,
        //HybridCache cache,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        var auction = await repository.GetAuctionModelById(id);
        
        if(auction is null) return Results.NotFound();
        if(auction.Seller != httpContext.User.Identity.Name) return Results.Forbid();
        
        repository.DeleteAuction(auction);
        //await cache.RemoveAsync($"auctions-{id}", cancellationToken);
        
        await publishEndpoint.Publish(new AuctionDeleted { Id = auction.Id }, cancellationToken);
        var result = await repository.SaveChangesAsync();
        
        return result ? Results.NoContent() : Results.BadRequest("Could not save changes to DB");
    }
}