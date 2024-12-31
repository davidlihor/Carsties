using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace BiddingService.Models;

[Collection("Auctions")]
public class Auction
{
    [BsonId]
    public Guid Id { get; set; }
    public DateTime AuctionEnd { get; set; }
    public string Seller { get; set; }
    public int ReservePrice { get; set; }
    public bool Finished { get; set; }
}