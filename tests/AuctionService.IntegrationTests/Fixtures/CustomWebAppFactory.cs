using AuctionService.Data;
using AuctionService.Data.Interceptors;
using AuctionService.IntegrationTests.Util;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace AuctionService.IntegrationTests.Fixtures;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveDbContext<DataContext>();
            services.AddDbContext<DataContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityIntercept>());
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });
            services.EnsureDbCreated<DataContext>();

            services.AddMassTransitTestHarness();
            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer(options =>
            {
                options.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
            });
        });
    }

    public Task DisposeAsync() => _postgreSqlContainer.DisposeAsync().AsTask();
}