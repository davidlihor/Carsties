using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionControllerTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private const string GuidId = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetAuctions_ShouldReturnAuctions()
    {
        // arrange
        
        // act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");
        
        // assert
        Assert.Equal(3, response.Count);
    }
    
    [Fact]
    public async Task GetAuctionById_WithValidId_ShouldReturnAuction()
    {
        // arrange
        
        // act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{GuidId}");
        
        // assert
        Assert.Equal("GT", response.Model);
    }
    
    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShouldReturnNotFound()
    {
        // arrange
        
        // act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");
        
        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithNoAuth_ShouldReturnUnauthorized()
    {
        // arrange
        var auction = new CreateAuctionDto { Make = "test" };
        
        // act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);
        
        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithAuth_ShouldReturnCreated()
    {
        // arrange
        var auction = DtoHelper.GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);
        
        // assert
        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        
        response.EnsureSuccessStatusCode();
        Assert.Equal("bob", createdAuction.Seller);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
    
    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturnBadRequest()
    {
        // arrange
        var auction = DtoHelper.GetAuctionForCreate();
        auction.Make = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);
        
        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateAuction_WithValidUpdateAuctionAndUser_ShouldReturnNoContent()
    {
        // arrange
        var auction = new UpdateAuctionDto{ Make = "Updated" };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GuidId}", auction);
        
        // assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task UpdateAuction_WithValidUpdateAuctionAndInvalidUser_ShouldReturnForbidden()
    {
        // arrange
        var auction = new UpdateAuctionDto{ Make = "Updated" };
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("alice"));
        
        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GuidId}", auction);
        
        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}