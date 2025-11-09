using BookStore.Api.Models;
using Npgsql;

namespace BookStore.Api.Data;

public class PostgresCatalogRepository : ICatalogRepository
{
    private const string TableName = "catalog_items";
    private readonly NpgsqlDataSource _dataSource;
    private readonly object _initLock = new();
    private bool _initialized;

    public PostgresCatalogRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        EnsureInitialized();
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
            CreatedAtUtc = DateTime.UtcNow
        };

        using var connection = _dataSource.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @$"
            insert into {TableName}
            (id, title, description, category, price, duration_minutes, created_at_utc)
            values (@id, @title, @description, @category, @price, @duration_minutes, @created_at_utc);";

        command.Parameters.AddWithValue("@id", item.Id);
        command.Parameters.AddWithValue("@title", item.Title);
        command.Parameters.AddWithValue("@description", (object?)item.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@category", item.Category.ToString());
        command.Parameters.AddWithValue("@price", item.Price);
        command.Parameters.AddWithValue("@duration_minutes", item.DurationMinutes);
        command.Parameters.AddWithValue("@created_at_utc", item.CreatedAtUtc);

        command.ExecuteNonQuery();
        return item;
    }

    public bool Delete(Guid id)
    {
        using var connection = _dataSource.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @$"delete from {TableName} where id = @id;";
        command.Parameters.AddWithValue("@id", id);

        var affected = command.ExecuteNonQuery();
        return affected > 0;
    }

    public CatalogItem? GetById(Guid id)
    {
        using var connection = _dataSource.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @$"
            select id, title, description, category, price, duration_minutes, created_at_utc
            from {TableName}
            where id = @id;";
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return Map(reader);
    }

    public IEnumerable<CatalogItem> GetItems(CatalogItemCategory? category = null)
    {
        using var connection = _dataSource.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @$"
            select id, title, description, category, price, duration_minutes, created_at_utc
            from {TableName}
            {(category is null ? string.Empty : "where category = @category")}
            order by title asc;";

        if (category is not null)
        {
            command.Parameters.AddWithValue("@category", category.Value.ToString());
        }

        using var reader = command.ExecuteReader();
        var results = new List<CatalogItem>();
        while (reader.Read())
        {
            results.Add(Map(reader));
        }

        return results;
    }

    public CatalogItem Update(Guid id, UpdateCatalogItemInput input)
    {
        var existing = GetById(id) ?? throw new KeyNotFoundException($"Item {id} was not found.");
        var updated = existing with
        {
            Title = string.IsNullOrWhiteSpace(input.Title)
                ? existing.Title
                : input.Title.Trim(),
            Description = input.Description?.Trim() ?? existing.Description,
            Category = input.Category ?? existing.Category,
            Price = input.Price ?? existing.Price,
            DurationMinutes = input.DurationMinutes ?? existing.DurationMinutes
        };

        using var connection = _dataSource.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @$"
            update {TableName}
            set title = @title,
                description = @description,
                category = @category,
                price = @price,
                duration_minutes = @duration_minutes
            where id = @id;";

        command.Parameters.AddWithValue("@id", id);
        command.Parameters.AddWithValue("@title", updated.Title);
        command.Parameters.AddWithValue("@description", (object?)updated.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@category", updated.Category.ToString());
        command.Parameters.AddWithValue("@price", updated.Price);
        command.Parameters.AddWithValue("@duration_minutes", updated.DurationMinutes);

        var affected = command.ExecuteNonQuery();
        if (affected == 0)
        {
            throw new KeyNotFoundException($"Item {id} was not found.");
        }

        return updated;
    }

    private void EnsureInitialized()
    {
        if (_initialized)
        {
            return;
        }

        lock (_initLock)
        {
            if (_initialized)
            {
                return;
            }

            using var connection = _dataSource.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @$"
                create table if not exists {TableName} (
                    id uuid primary key,
                    title text not null,
                    description text null,
                    category text not null,
                    price numeric(12,2) not null,
                    duration_minutes integer not null default 0,
                    created_at_utc timestamptz not null default (now() at time zone 'utc')
                );
                create index if not exists idx_{TableName}_category on {TableName}(category);
            ";
            command.ExecuteNonQuery();

            SeedIfEmpty(connection);
            _initialized = true;
        }
    }

    private static CatalogItem Map(NpgsqlDataReader reader)
    {
        return new CatalogItem
        {
            Id = reader.GetGuid(0),
            Title = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            Category = Enum.Parse<CatalogItemCategory>(reader.GetString(3), ignoreCase: true),
            Price = reader.GetDecimal(4),
            DurationMinutes = reader.GetInt32(5),
            CreatedAtUtc = reader.GetDateTime(6).ToUniversalTime()
        };
    }

    private static void SeedIfEmpty(NpgsqlConnection connection)
    {
        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = @$"select 1 from {TableName} limit 1;";
        var hasRows = checkCommand.ExecuteScalar() is not null;
        if (hasRows)
        {
            return;
        }

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
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        foreach (var item in sampleItems)
        {
            using var insert = connection.CreateCommand();
            insert.CommandText = @$"
                insert into {TableName}
                (id, title, description, category, price, duration_minutes, created_at_utc)
                values (@id, @title, @description, @category, @price, @duration_minutes, @created_at_utc);";

            insert.Parameters.AddWithValue("@id", item.Id);
            insert.Parameters.AddWithValue("@title", item.Title);
            insert.Parameters.AddWithValue("@description", (object?)item.Description ?? DBNull.Value);
            insert.Parameters.AddWithValue("@category", item.Category.ToString());
            insert.Parameters.AddWithValue("@price", item.Price);
            insert.Parameters.AddWithValue("@duration_minutes", item.DurationMinutes);
            insert.Parameters.AddWithValue("@created_at_utc", item.CreatedAtUtc);

            insert.ExecuteNonQuery();
        }
    }
}
