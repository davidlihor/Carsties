using AuctionService.DTOs;

namespace AuctionService.IntegrationTests.Util;

public static class DtoHelper
{
    public static CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "test",
            ImageUrl = "test",
            Color = "test",
            ReservePrice = 10,
            Mileage = 10,
            Year = 10
        };
    }
}