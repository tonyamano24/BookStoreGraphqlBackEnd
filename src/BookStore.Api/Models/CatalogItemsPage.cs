using System.Collections.Generic;

namespace BookStore.Api.Models;

public record CatalogItemsPage(
    IReadOnlyList<CatalogItem> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page * PageSize < TotalCount;

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
