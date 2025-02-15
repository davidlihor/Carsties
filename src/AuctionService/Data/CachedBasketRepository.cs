using AuctionService.DTOs;
using AuctionService.Models;
using AutoMapper;
using Microsoft.Extensions.Caching.Hybrid;

namespace AuctionService.Data;

public class CachedBasketRepository(IAuctionRepository repository, IMapper mapper, HybridCache cache) : IAuctionRepository
{
    public async Task<List<AuctionDto>> GetAuctionsAsync(string date)
    {
        return await repository.GetAuctionsAsync(date);
    }

    public async Task<AuctionDto> GetAuctionByIdAsync(Guid id)
    {
        return await cache.GetOrCreateAsync($"auctions-{id}", async _ => 
            await repository.GetAuctionByIdAsync(id), null, ["auctions"]);
    }

    public async Task<Auction> GetAuctionModelById(Guid id)
    {
        return await cache.GetOrCreateAsync($"auctions-models-{id}", async _ =>
            await repository.GetAuctionModelById(id), null, ["auctions-models"]);
    }

    public void CreateAuction(Auction auction)
    {
        repository.CreateAuction(auction);
        //await cache.SetAsync($"auctions-{auction.Id}", mapper.Map<AuctionDto>(auction), null, ["auctions"]);
    }

    public async void UpdateAuction(Auction auction)
    {
        repository.UpdateAuction(auction);
        await cache.RemoveAsync($"auctions-{auction.Id}");
    }

    public async void DeleteAuction(Auction auction)
    {
        repository.DeleteAuction(auction);
        await cache.RemoveAsync($"auctions-{auction.Id}");
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await repository.SaveChangesAsync();
    }
}
