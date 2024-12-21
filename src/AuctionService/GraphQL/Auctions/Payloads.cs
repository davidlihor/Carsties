using AuctionService.DTOs;

namespace AuctionService.GraphQL.Auctions;

public record CreateAuctionPayload
{
    public bool Success { get; set; } = true;
    public AuctionDto Auction { get; set; }
    public string Message { get; set; } = "Auction created successfully";
}

public record UpdateAuctionPayload
{
    public bool Success { get; set; } = true;
    public AuctionDto Auction { get; set; }
    public string Message { get; set; } = "Auction updated successfully";
}

public record DeleteAuctionPayload
{
    public bool Success { get; set; } = true;
    public Guid AuctionId { get; set; }
    public string Message { get; set; } = "Auction deleted successfully";
}