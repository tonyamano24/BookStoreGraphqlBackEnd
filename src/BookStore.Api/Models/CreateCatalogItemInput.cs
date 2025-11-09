namespace BookStore.Api.Models;

public record CreateCatalogItemInput(
    string Title,
    string? Description,
    CatalogItemCategory Category,
    decimal Price,
    int DurationMinutes,
    string? ImageUrl);
