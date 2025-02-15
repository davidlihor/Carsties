using AuctionService.DTOs;
using AuctionService.Models;

namespace AuctionService.Data;

public interface IAuctionRepository
{
    Task<List<AuctionDto>> GetAuctionsAsync(string date);
    Task<AuctionDto> GetAuctionByIdAsync(Guid id);
    Task<Auction> GetAuctionModelById(Guid id);
    void CreateAuction(Auction auction);
    void UpdateAuction(Auction auction);
    void DeleteAuction(Auction auction);
    Task<bool> SaveChangesAsync();
}
