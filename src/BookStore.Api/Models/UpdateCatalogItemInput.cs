namespace BookStore.Api.Models;

public record UpdateCatalogItemInput(
    string? Title,
    string? Description,
    CatalogItemCategory? Category,
    decimal? Price,
    int? DurationMinutes,
    string? ImageUrl);
