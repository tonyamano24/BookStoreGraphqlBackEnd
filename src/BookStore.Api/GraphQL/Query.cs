using BookStore.Api.Data;
using BookStore.Api.Models;
using HotChocolate;

namespace BookStore.Api.GraphQL;

public class Query
{
    public IEnumerable<CatalogItem> GetCatalogItems(
        CatalogItemCategory? category,
        [Service] ICatalogRepository repository)
    {
        return repository.GetItems(category);
    }

    public CatalogItem? GetCatalogItemById(Guid id, [Service] ICatalogRepository repository)
    {
        return repository.GetById(id);
    }

    public CatalogItemsPage GetCatalogItemsPage(
        CatalogItemCategory? category,
        [Service] ICatalogRepository repository,
        int page = 1,
        int pageSize = -1)
    {
        page = Math.Max(1, page);
        int? normalizedPageSize = pageSize > 0 ? pageSize : null;
        return repository.GetItemsPage(category, page, normalizedPageSize);
    }
}
