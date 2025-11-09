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
}
