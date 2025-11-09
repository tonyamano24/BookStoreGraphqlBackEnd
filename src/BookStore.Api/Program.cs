using BookStore.Api.Data;
using BookStore.Api.GraphQL;
using BookStore.Api.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CatalogDb")
    ?? builder.Configuration["POSTGRES_CONNECTION_STRING"];

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddSingleton(_ => NpgsqlDataSource.Create(connectionString!));
    builder.Services.AddSingleton<ICatalogRepository, PostgresCatalogRepository>();
}
else
{
    builder.Services.AddSingleton<ICatalogRepository, InMemoryCatalogRepository>();
}

builder.Services.AddSingleton<CatalogItemDetailProvider>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<CatalogItemNode>()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/graphql"));

app.MapGraphQL("/graphql");

app.Run();
