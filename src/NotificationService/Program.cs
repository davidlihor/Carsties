using MassTransit;
using NotificationService.Hubs;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddMassTransit(options =>
{
    options.AddConsumers(typeof(Program).Assembly);
    options.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notification", false));
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

var app = builder.Build();
app.MapHub<NotificationHub>("/notification");
app.UseMetricServer();
app.UseHttpMetrics();
app.Run();
