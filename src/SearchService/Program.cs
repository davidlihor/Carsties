using System.Net;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;   

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionServiceHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddMassTransit(options =>
{
    options.AddConsumers(typeof(Program).Assembly);
    options.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    options.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration["RabbitMQ:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:UserName", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });
        config.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });
        config.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

//app.UseAuthorization();
app.MapControllers();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();

//return;
static IAsyncPolicy<HttpResponseMessage> GetPolicy() => HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

public partial class Program {}