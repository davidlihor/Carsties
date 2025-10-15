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
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Product);
        CreateMap<Product, AuctionDto>();

        CreateMap<Auction, AuctionCreated>().IncludeMembers(x => x.Product);
        CreateMap<Product, AuctionCreated>();

        CreateMap<Auction, AuctionUpdated>().IncludeMembers(x => x.Product);
        CreateMap<Product, AuctionUpdated>();


        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Product>();

        CreateMap<UpdateAuctionDto, Auction>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s));
        CreateMap<UpdateAuctionDto, Product>();


        CreateMap<CreateAuctionInput, Auction>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionInput, Product>();

        CreateMap<UpdateAuctionInput, Auction>()
            .ForMember(d => d.Product, o => o.MapFrom(s => s));
        CreateMap<UpdateAuctionInput, Product>();
    }
}