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
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Fullstack GraphQL Bootcamp",
                Description = "Two-day bootcamp walking through schema-first thinking, resolvers, and deployment.",
                Category = CatalogItemCategory.Course,
                DurationMinutes = 210,
                Price = 249.00m,
                ImageUrl = "https://placehold.co/600x400?text=Fullstack+Bootcamp",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Reactive GraphQL Cookbook",
                Description = "40+ recipes for building reactive GraphQL clients with modern frameworks.",
                Category = CatalogItemCategory.Book,
                DurationMinutes = 0,
                Price = 34.90m,
                ImageUrl = "https://placehold.co/600x400?text=Reactive+Cookbook",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Federation Deep Dive Live",
                Description = "Live coding session on slicing monolith schemas into federated subgraphs.",
                Category = CatalogItemCategory.Course,
                DurationMinutes = 150,
                Price = 189.00m,
                ImageUrl = "https://placehold.co/600x400?text=Federation+Live",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Schema Design Cheatsheet",
                Description = "Printable cheatsheet covering naming, connections, and pagination patterns.",
                Category = CatalogItemCategory.Book,
                DurationMinutes = 0,
                Price = 19.00m,
                ImageUrl = "https://placehold.co/600x400?text=Schema+Cheatsheet",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "GraphQL Sticker Pack",
                Description = "Set of 10 holographic stickers for laptops and workshop giveaways.",
                Category = CatalogItemCategory.Merchandise,
                DurationMinutes = 0,
                Price = 9.90m,
                ImageUrl = "https://placehold.co/600x400?text=Sticker+Pack",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Workshop Swag Bundle",
                Description = "Tote bag, notebook, and enamel pin combo for event attendees.",
                Category = CatalogItemCategory.Merchandise,
                DurationMinutes = 0,
                Price = 59.90m,
                ImageUrl = "https://placehold.co/600x400?text=Swag+Bundle",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Realtime APIs with GraphQL",
                Description = "Hands-on lab covering subscriptions, live queries, and streaming transport.",
                Category = CatalogItemCategory.Course,
                DurationMinutes = 180,
                Price = 209.00m,
                ImageUrl = "https://placehold.co/600x400?text=Realtime+APIs",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "GraphQL Testing Toolkit",
                Description = "Guide and templates for snapshot, integration, and contract testing.",
                Category = CatalogItemCategory.Book,
                DurationMinutes = 0,
                Price = 27.50m,
                ImageUrl = "https://placehold.co/600x400?text=Testing+Toolkit",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Schema Explorer Poster",
                Description = "A2 poster showing resolver pipelines and common schema directives.",
                Category = CatalogItemCategory.Merchandise,
                DurationMinutes = 0,
                Price = 14.50m,
                ImageUrl = "https://placehold.co/600x400?text=Schema+Poster",
                CreatedAtUtc = DateTime.UtcNow
            },
            new CatalogItem
            {
                Id = Guid.NewGuid(),
                Title = "Production GraphQL Observability",
                Description = "Mini-course on tracing, logging, and profiling GraphQL workloads.",
                Category = CatalogItemCategory.Course,
                DurationMinutes = 120,
                Price = 199.00m,
                ImageUrl = "https://placehold.co/600x400?text=GraphQL+Observability",
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        foreach (var item in sampleItems)
        {
            _items[item.Id] = item;
        }
    }
}
