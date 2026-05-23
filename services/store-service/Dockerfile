FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Store.API/Store.API.csproj", "src/Store.API/"]
COPY ["src/Store.Application/Store.Application.csproj", "src/Store.Application/"]
COPY ["src/Store.Domain/Store.Domain.csproj", "src/Store.Domain/"]
COPY ["src/Store.Infrastructure/Store.Infrastructure.csproj", "src/Store.Infrastructure/"]

RUN dotnet restore "src/Store.API/Store.API.csproj"

COPY . .

RUN dotnet publish "src/Store.API/Store.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.API.dll"]