using AuctionService.DTOs;
using AuctionService.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data;

public class AuctionRepository(DataContext dataContext, IMapper mapper) : IAuctionRepository
{
    public async Task<List<AuctionDto>> GetAuctionsAsync(string date)
    {
        var query = dataContext.Auctions.OrderBy(x => x.Product.Make).AsQueryable();

        if (!string.IsNullOrWhiteSpace(date))
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);

        return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<AuctionDto> GetAuctionByIdAsync(Guid id)
    {
        return await dataContext.Auctions
            .ProjectTo<AuctionDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Auction> GetAuctionModelById(Guid id)
    {
        return await dataContext.Auctions
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public void CreateAuction(Auction auction)
    {
        dataContext.Auctions.Add(auction);
    }

    public void UpdateAuction(Auction auction)
    {
        dataContext.Auctions.Update(auction);
    }

    public void DeleteAuction(Auction auction)
    {
        dataContext.Auctions.Remove(auction);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
}
