# Repository Guidelines

## Project Structure & Module Organization
`src/BookStore.Api` hosts the entire service. `Program.cs` wires dependency injection and the Hot Chocolate server; `Data/` keeps repository contracts plus the in-memory implementation; `Models/` stores DTOs and enums consumed by resolvers; `GraphQL/` contains `Query`, `Mutation`, and type extensions. When adding features, update the layers in that order (model → persistence → resolver) so reviewers can follow the data flow.

## Build, Test, and Development Commands
- `dotnet restore ./src/BookStore.Api/BookStore.Api.csproj` hydrates NuGet dependencies.
- `dotnet build ./src/BookStore.Api/BookStore.Api.csproj -c Release` ensures the backend compiles before pushing.
- `dotnet run --project ./src/BookStore.Api/BookStore.Api.csproj` serves GraphQL at `/graphql` on localhost.
- `dotnet watch run --project ./src/BookStore.Api/BookStore.Api.csproj` hot-reloads while iterating on resolvers.
- `dotnet test` (once test projects exist under `src`) executes every xUnit test discovered in the tree.

## Coding Style & Naming Conventions
Stick to .NET 8 minimal APIs, file-scoped namespaces, and 4-space indentation. Use PascalCase for public members, camelCase for locals, and explicit return types on resolvers so schema inference stays predictable. Favor `record` types for request/response payloads, inject dependencies via `[Service]`, and run `dotnet format` before committing to keep analyzers happy.

## Testing Guidelines
Adopt xUnit with Hot Chocolate snapshot or request tests so schema deltas are intentional. Name tests `Method_Scenario_Result`, mock the repository instead of touching the in-memory data, and ensure every new Query/Mutation path asserts both payload shape and repository interaction. Target smoke coverage for the happy path plus at least one failure case per resolver.

## Commit & Pull Request Guidelines
Recent history uses short, imperative summaries (`Add GraphQL backend skeleton`, `Create readme.md`). Keep the subject under ~72 characters, add body bullets for rationale, and mention any schema-breaking change explicitly. PRs should link issues, describe the user-facing GraphQL impact, list new commands/config, and attach screenshots or curl snippets when helpful for reviewers.

## Security & Configuration Tips
The app currently serves mock data, but the moment you connect real stores, load secrets via `dotnet user-secrets` or environment variables instead of hardcoding. Double-check CORS and `/graphql` accessibility before merging, and document any new appsettings keys inside the PR description so workshop facilitators can reproduce the environment quickly.
