using AuctionService.Data;
using AuctionService.Data.Interceptors;
using AuctionService.GraphQL.Auctions;
using AuctionService.Services;
using Carter;
using HotChocolate.AspNetCore.Voyager;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Npgsql;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter();
builder.Services.AddGrpc();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditableEntityIntercept>();
builder.Services.AddDbContext<DataContext>((serviceProvider, options) =>
{
    options.AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityIntercept>());
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});
#pragma warning disable EXTEXP0018
builder.Services.AddHybridCache(options =>
{
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        LocalCacheExpiration = TimeSpan.FromMinutes(5),
        Expiration = TimeSpan.FromMinutes(5)
    };
});
#pragma warning restore EXTEXP0018
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "AuctionService";
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(options =>
{
    options.AddEntityFrameworkOutbox<DataContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });
    options.AddConsumers(typeof(Program).Assembly);
    options.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
    options.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration["RabbitMQ:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:UserName", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });
        config.ConfigureEndpoints(context);
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["IdentityServerUrl"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.NameClaimType = "username";
});
builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<AuctionQuery>()
    .AddMutationType<AuctionMutation>()
    .AddSubscriptionType<AuctionSubscription>()
    .AddType<AuctionType>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddInMemorySubscriptions()
    .AddAuthorization();

builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
//builder.Services.Decorate<IAuctionRepository, CachedBasketRepository>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GrpcAuctionService>();
app.MapCarter();
app.MapGraphQL();
app.MapNitroApp();
app.UseVoyager("/graphql", "/ui/voyager");

var retryPolicy = Policy
    .Handle<NpgsqlException>()
    .WaitAndRetry(5, retryCount => TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
retryPolicy.ExecuteAndCapture(() => DbInitializer.InitDb(app));

app.Run();

public partial class Program {}