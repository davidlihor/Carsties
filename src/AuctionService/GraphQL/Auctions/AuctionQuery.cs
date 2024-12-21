using AuctionService.Data;
using AuctionService.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace AuctionService.GraphQL.Auctions;

public class AuctionQuery(IMapper mapper)
{
    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<AuctionDto> GetAuctions([Service] DataContext context)
    {
        return context.Auctions.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).AsQueryable();
    }
}