using BookStore.Api.Models;
using HotChocolate;
using HotChocolate.Types;

namespace BookStore.Api.GraphQL;

[ExtendObjectType(typeof(CatalogItem))]
public class CatalogItemNode
{
    public string CategoryDisplayName([Parent] CatalogItem item)
        => item.Category switch
        {
            CatalogItemCategory.Book => "Book",
            CatalogItemCategory.Course => "Course",
            CatalogItemCategory.Merchandise => "Merchandise",
            _ => item.Category.ToString()
        };

    public double DurationHours([Parent] CatalogItem item)
        => Math.Round(item.DurationMinutes / 60d, 2);
}
