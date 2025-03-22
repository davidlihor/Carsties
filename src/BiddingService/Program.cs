using BiddingService.Data;
using BiddingService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<GrpcAuctionClient>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMongoDB(builder.Configuration.GetConnectionString("MongoDB")!,"BidDb");
});
builder.Services.AddHostedService<CheckAuctionFinished>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(options =>
{
    options.AddConsumers(typeof(Program).Assembly);
    options.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));
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
    options.RequireHttpsMetadata = false;
    options.Authority = builder.Configuration["JWT:Authority"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        ClockSkew = TimeSpan.FromMinutes(5),
        NameClaimType = "preferred_username"
    };
});

var app = builder.Build();
app.MapControllers();
app.UseMetricServer();
app.UseHttpMetrics();
await app.InitDbAsync();
app.Run();
