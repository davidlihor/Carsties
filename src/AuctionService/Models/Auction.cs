using AuctionService.Models.Abstractions;

namespace AuctionService.Models;

public class Auction : Entity<Guid>
{
    public int ReservePrice { get; set; }
    public string Seller { get; set; }
    public string Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentHighBid { get; set; }
    public DateTime AuctionEnd { get; set; }
    public Status Status { get; set; }
    public Product Product { get; set; }
}