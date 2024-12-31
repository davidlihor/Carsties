namespace AuctionService.Models.Abstractions;

public interface IAuditable<T> : IAuditable
{
    public T Id { get; set; }
}

public interface IAuditable
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}