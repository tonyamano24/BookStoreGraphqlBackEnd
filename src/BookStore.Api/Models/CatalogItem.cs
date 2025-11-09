namespace BookStore.Api.Models;

public record CatalogItem
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public CatalogItemCategory Category { get; init; }

    public decimal Price { get; init; }

    public int DurationMinutes { get; init; }

    public string? ImageUrl { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
