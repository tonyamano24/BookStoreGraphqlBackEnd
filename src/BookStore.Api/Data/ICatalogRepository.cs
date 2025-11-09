using BookStore.Api.Models;

namespace BookStore.Api.Data;

public interface ICatalogRepository
{
    IEnumerable<CatalogItem> GetItems(CatalogItemCategory? category = null);

    CatalogItem? GetById(Guid id);

    CatalogItem Add(CreateCatalogItemInput input);

    CatalogItem Update(Guid id, UpdateCatalogItemInput input);

    bool Delete(Guid id);

    CatalogItemsPage GetItemsPage(CatalogItemCategory? category, int page, int? pageSize);
}
