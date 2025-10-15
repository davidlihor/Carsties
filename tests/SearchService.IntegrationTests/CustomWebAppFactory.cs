using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using Xunit;
using Testcontainers.MongoDb;

namespace SearchService.IntegrationTests;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;

    public CustomWebAppFactory()
    {
        _mongoContainer = new MongoDbBuilder()
          .WithImage("mongo:6.0")
          .Build();
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();

        await DB.InitAsync("testDB", MongoClientSettings.FromConnectionString(_mongoContainer.GetConnectionString()));

        await DB.Index<Item>()
          .Key(x => x.Make, KeyType.Text)
          .Key(x => x.Model, KeyType.Text)
          .Key(x => x.Color, KeyType.Text)
          .CreateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.AddMassTransitTestHarness();
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }
}