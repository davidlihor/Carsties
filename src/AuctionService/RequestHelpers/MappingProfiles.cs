using AuctionService.DTOs;
using AuctionService.GraphQL.Auctions;
using AuctionService.Models;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();

        CreateMap<Auction, AuctionCreated>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionCreated>();
        
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionUpdated>();
        
        
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
        
        CreateMap<UpdateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<UpdateAuctionDto, Item>();
        
        
        CreateMap<CreateAuctionInput, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionInput, Item>();
        
        CreateMap<UpdateAuctionInput, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<UpdateAuctionInput, Item>();
    }
}