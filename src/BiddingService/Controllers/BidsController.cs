using AutoMapper;
using BiddingService.Data;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController(
    AppDbContext context,
    IMapper mapper,
    GrpcAuctionClient grpcClient,
    IPublishEndpoint publishEndpoint) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BidDto>> PlaceBid(Guid auctionId, int amount)
    {
        var auction = await context.Auctions.FindAsync(auctionId) ?? grpcClient.GetAuction(auctionId);
        if (auction is null) return BadRequest("Cannot accept bids on this auction");

        if (auction.Seller == User.Identity?.Name)
            return BadRequest("You cannot bid on your own auction");

        var bid = new Bid
        {
            Amount = amount,
            AuctionId = auctionId,
            Bidder = User.Identity?.Name
        };

        if (auction.AuctionEnd < DateTime.UtcNow)
        {
            bid.BidStatus = BidStatus.Finished;
        }
        else
        {
            var highBid = await context.Bids
                .Where(x => x.AuctionId == auctionId)
                .OrderByDescending(x => x.Amount)
                .FirstOrDefaultAsync();

            if (highBid is not null && amount > highBid.Amount || highBid is null)
            {
                bid.BidStatus = amount > auction.ReservePrice ? BidStatus.Accepted : BidStatus.AcceptedBelowReserved;
            }

            if (highBid is not null && amount <= highBid.Amount)
            {
                bid.BidStatus = BidStatus.TooLow;
            }
        }
        await context.Bids.AddAsync(bid);
        await context.SaveChangesAsync();
        await publishEndpoint.Publish(mapper.Map<BidPlaced>(bid));

        return Ok(mapper.Map<BidDto>(bid));
    }

    [HttpGet("{auctionId:guid}")]
    public async Task<ActionResult<List<BidDto>>> GetBidsForAuction(Guid auctionId)
    {
        var bids = await context.Bids
            .Where(x => x.AuctionId == auctionId)
            .OrderByDescending(x => x.BidTime)
            .ToListAsync();

        return bids.Select(mapper.Map<BidDto>).ToList();
    }
}