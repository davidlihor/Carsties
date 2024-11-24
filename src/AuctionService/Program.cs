using AuctionService.Data;
using AuctionService.GraphQL;
using Carter;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCarter();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"));
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
var app = builder.Build();
app.MapGraphQL();

app.MapControllers();
app.MapCarter();

app.UseAuthorization();

try
{
    DbInitializer.InitDB(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();