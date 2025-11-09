using BookStore.Api.Data;
using BookStore.Api.GraphQL;
using BookStore.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<ICatalogRepository, InMemoryCatalogRepository>();

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
