using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuctionService.Controllers;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Models;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace AuctionService.UnitTests;

public class AuctionControllerTests
{
    private readonly Mock<IAuctionRepository> _repository;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Fixture _fixture;
    private readonly IMapper _mapper;
    private readonly DefaultHttpContext _httpContext;
    private readonly CancellationToken _ct;

    public AuctionControllerTests()
    {
        _fixture = new Fixture();
        _repository = new Mock<IAuctionRepository>();
        _publishEndpoint = new Mock<IPublishEndpoint>();

        var mockMapper = new MapperConfiguration(config =>
        {
            config.AddMaps(typeof(MappingProfiles).Assembly);

        }).CreateMapper().ConfigurationProvider;
        _mapper = new Mapper(mockMapper);

        _httpContext = new DefaultHttpContext
        {
            User = Helpers.GetClaimsPrincipal()
        };
        _ct = CancellationToken.None;
    }

    [Fact]
    public async Task GetAuctions_WithNoParams_ReturnsAuctions()
    {
        // arrange
        var auctions = _fixture.CreateMany<AuctionDto>(2).ToList();
        _repository.Setup(x => x.GetAuctionsAsync(null)).ReturnsAsync(auctions);

        // act
        var result = await AuctionsEndpoints.GetAuctions(null, _repository.Object);

        // assert
        Assert.Equal(2, auctions.Count);
        Assert.IsType<Ok<List<AuctionDto>>>(result);
    }

    [Fact]
    public async Task GetAuction_WithValidGuid_ReturnsAuction()
    {
        // arrange
        var auction = _fixture.Create<AuctionDto>();
        _repository.Setup(x => x.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

        // act
        var result = await AuctionsEndpoints.GetAuction(_repository.Object, auction.Id, _ct);
        var response = result as Ok<AuctionDto>;

        // assert
        Assert.Equal(auction.Make, response.Value.Make);
        Assert.IsType<Ok<AuctionDto>>(result);
    }

    [Fact]
    public async Task GetAuction_WithInvalidGuid_ReturnsNotFound()
    {
        // arrange
        _repository.Setup(x => x.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

        // act
        var result = await AuctionsEndpoints.GetAuction(_repository.Object, Guid.NewGuid(), _ct);

        // assert
        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task CreateAuction_WithValidCreateAuctionDto_ReturnsCreatedAtRoute()
    {
        // arrange
        var auction = _fixture.Create<CreateAuctionDto>();
        _repository.Setup(x => x.CreateAuction(It.IsAny<Auction>()));
        _repository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await AuctionsEndpoints.CreateAuction(auction, _repository.Object, _mapper, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as CreatedAtRoute<AuctionDto>;

        // assert
        Assert.NotNull(response.Value);
        Assert.Equal("GetAuction", response.RouteName);
        Assert.IsType<AuctionDto>(response.Value);
    }

    [Fact]
    public async Task CreateAuction_FailedSave_ReturnsBadRequest()
    {
        // arrange
        var auction = _fixture.Create<CreateAuctionDto>();
        _repository.Setup(x => x.CreateAuction(It.IsAny<Auction>()));
        _repository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(false);

        // act
        var result = await AuctionsEndpoints.CreateAuction(auction, _repository.Object, _mapper, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as BadRequest<string>;

        // assert
        Assert.IsType<BadRequest<string>>(response);
    }

    [Fact]
    public async Task UpdateAuction_WithUpdateAuctionDto_ReturnsNoContent()
    {
        // arrange
        var auctionDto = _fixture.Create<UpdateAuctionDto>();

        var auction = _fixture.Build<Auction>().Without(x => x.Product).Create();
        auction.Product = _fixture.Build<Product>().Without(x => x.Auction).Create();
        auction.Seller = "user";

        _repository.Setup(x => x.GetAuctionModelById(It.IsAny<Guid>())).ReturnsAsync(auction);
        _repository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await AuctionsEndpoints.UpdateAuction(auction.Id, auctionDto, _repository.Object, _mapper, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as NoContent;

        // assert
        Assert.IsType<NoContent>(response);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidUser_ReturnsForbid()
    {
        // arrange
        var auctionDto = _fixture.Create<UpdateAuctionDto>();
        var auction = _fixture.Build<Auction>().Without(x => x.Product).Create();

        _repository.Setup(x => x.GetAuctionModelById(It.IsAny<Guid>())).ReturnsAsync(auction);

        // act
        var result = await AuctionsEndpoints.UpdateAuction(auction.Id, auctionDto, _repository.Object, _mapper, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as ForbidHttpResult;

        // assert
        Assert.IsType<ForbidHttpResult>(response);
    }

    [Fact]
    public async Task UpdateAuction_WithInvalidGuid_ReturnsForbid()
    {
        // arrange
        var auctionDto = _fixture.Create<UpdateAuctionDto>();
        var auction = _fixture.Build<Auction>().Without(x => x.Product).Create();

        _repository.Setup(x => x.GetAuctionModelById(It.IsAny<Guid>())).ReturnsAsync(value: null);

        // act
        var result = await AuctionsEndpoints.UpdateAuction(auction.Id, auctionDto, _repository.Object, _mapper, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as NotFound;

        // assert
        Assert.IsType<NotFound>(response);
    }

    [Fact]
    public async Task DeleteAuction_WithValidUser_ReturnsNoContent()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Product).Create();
        auction.Seller = "user";

        _repository.Setup(x => x.GetAuctionModelById(It.IsAny<Guid>())).ReturnsAsync(auction);
        _repository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

        // act
        var result = await AuctionsEndpoints.DeleteAuction(Guid.NewGuid(), _repository.Object, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as NoContent;

        // assert
        Assert.IsType<NoContent>(response);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidGuid_ReturnsNotFound()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Product).Create();
        _repository.Setup(x => x.GetAuctionModelById(It.IsAny<Guid>())).ReturnsAsync(value: null);

        // act
        var result = await AuctionsEndpoints.DeleteAuction(auction.Id, _repository.Object, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as NotFound;

        // assert
        Assert.IsType<NotFound>(response);
    }

    [Fact]
    public async Task DeleteAuction_WithInvalidUser_ReturnsForbid()
    {
        // arrange
        var auction = _fixture.Build<Auction>().Without(x => x.Product).Create();
        _repository.Setup(x => x.GetAuctionModelById(It.IsAny<Guid>())).ReturnsAsync(auction);

        // act
        var result = await AuctionsEndpoints.DeleteAuction(auction.Id, _repository.Object, _httpContext, _publishEndpoint.Object, _ct);
        var response = result as ForbidHttpResult;

        // assert
        Assert.IsType<ForbidHttpResult>(response);
    }
}
