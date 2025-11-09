using BookStore.Api.Data;
using BookStore.Api.Models;
using HotChocolate;

namespace BookStore.Api.GraphQL;

public class Mutation
{
    public CatalogItem CreateCatalogItem(CreateCatalogItemInput input, [Service] ICatalogRepository repository)
    {
        var item = repository.Add(input);
        return item;
    }

    public CatalogItem UpdateCatalogItem(Guid id, UpdateCatalogItemInput input, [Service] ICatalogRepository repository)
    {
        return repository.Update(id, input);
    }

    public DeleteCatalogItemPayload DeleteCatalogItem(Guid id, [Service] ICatalogRepository repository)
    {
        var removed = repository.Delete(id);
        return new DeleteCatalogItemPayload(removed);
    }
}
