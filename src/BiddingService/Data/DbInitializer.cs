using Microsoft.EntityFrameworkCore;
using Polly;

namespace BiddingService.Data;

public static class DbInitializer
{
    public static async Task InitDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await Policy.Handle<TimeoutException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            .ExecuteAsync(async () => await context.Auctions.AnyAsync());
    }
}