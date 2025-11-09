FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BookStore.Api/BookStore.Api.csproj", "src/BookStore.Api/"]
RUN dotnet restore "src/BookStore.Api/BookStore.Api.csproj"
COPY . .
RUN dotnet publish "src/BookStore.Api/BookStore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookStore.Api.dll"]
