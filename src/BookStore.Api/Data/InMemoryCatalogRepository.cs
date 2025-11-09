using System.Collections.Concurrent;
using System.Linq;
using BookStore.Api.Models;

namespace BookStore.Api.Data;

public class InMemoryCatalogRepository : ICatalogRepository
{
    private readonly ConcurrentDictionary<Guid, CatalogItem> _items = new();

    public InMemoryCatalogRepository()
    {
        Seed();
    }

    public CatalogItem Add(CreateCatalogItemInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Title))
        {
            throw new ArgumentException("Title is required", nameof(input));
        }

        var item = new CatalogItem
        {
            Id = Guid.NewGuid(),
            Title = input.Title.Trim(),
            Description = input.Description?.Trim(),
            Category = input.Category,
            Price = input.Price,
            DurationMinutes = input.DurationMinutes,
            ImageUrl = string.IsNullOrWhiteSpace(input.ImageUrl) ? null : input.ImageUrl.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _items[item.Id] = item;
        return item;
    }

    public bool Delete(Guid id)
    {
        return _items.TryRemove(id, out _);
    }

    public CatalogItem? GetById(Guid id)
    {
        return _items.TryGetValue(id, out var item) ? item : null;
    }

    public IEnumerable<CatalogItem> GetItems(CatalogItemCategory? category = null)
    {
        var query = _items.Values.AsEnumerable();
        if (category is not null)
        {
            query = query.Where(item => item.Category == category);
        }

        return query.OrderBy(item => item.Title).ToArray();
    }

    public CatalogItem Update(Guid id, UpdateCatalogItemInput input)
    {
        return _items.AddOrUpdate(
            id,
            _ => throw new KeyNotFoundException($"Item {id} was not found."),
            (_, existing) => existing with
            {
                Title = string.IsNullOrWhiteSpace(input.Title)
                    ? existing.Title
                    : input.Title.Trim(),
                Description = input.Description?.Trim() ?? existing.Description,
                Category = input.Category ?? existing.Category,
                Price = input.Price ?? existing.Price,
                DurationMinutes = input.DurationMinutes ?? existing.DurationMinutes,
                ImageUrl = input.ImageUrl switch
                {
                    null => existing.ImageUrl,
                    "" => null,
                    _ => input.ImageUrl.Trim()
                }
            });
    }

    private void Seed()
    {
        var sampleItems = new[]
        {
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Blazor Workshop: GraphQL Basics",
                Description = "A hands-on workshop covering GraphQL fundamentals for .NET developers.",
                Category = CatalogItemCategory.Course,
                DurationMinutes = 180,
                Price = 199.00m,
                ImageUrl = "https://placehold.co/600x400?text=GraphQL+Workshop",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "GraphQL for Beginners",
                Description = "An introductory e-book that explains GraphQL concepts with practical examples.",
                Category = CatalogItemCategory.Book,
                DurationMinutes = 0,
                Price = 29.90m,
                ImageUrl = "https://placehold.co/600x400?text=GraphQL+Book",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Developer Starter Kit",
                Description = "A bundle containing a notebook, stickers, and a mug for your study desk.",
                Category = CatalogItemCategory.Merchandise,
                DurationMinutes = 0,
                Price = 45.00m,
                ImageUrl = "https://placehold.co/600x400?text=Dev+Kit",
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        foreach (var item in sampleItems)
        {
            _items[item.Id] = item;
        }
    }
}
