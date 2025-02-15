using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionBusTests(CustomWebAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly ITestHarness _testHarness = factory.Services.GetTestHarness();
    
    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        DbHelper.ReinitDbForTests(db);
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreatedEvent()
    {
        // arrange
        var auction = DtoHelper.GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        
        // act
        var response = await _httpClient.PostAsJsonAsync("api/auctions", auction);
        
        // assert
        response.EnsureSuccessStatusCode();
        Assert.True(await _testHarness.Published.Any<AuctionCreated>());
    }
}