namespace AuctionService.GraphQL.Auctions;

public record CreateAuctionInput(string Make, string Model, int Year, string Color, int Mileage, string ImageUrl, int ReservePrice, DateTime AuctionEnd);
public record UpdateAuctionInput(Guid Id, string Make, string Model, int? Year, string Color, int? Mileage);
public record DeleteAuctionInput(Guid Id);