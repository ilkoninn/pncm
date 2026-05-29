FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY media-service/Protos/ media-service/Protos/

COPY ["store-service/src/Store.API/Store.API.csproj", "store-service/src/Store.API/"]
COPY ["store-service/src/Store.Application/Store.Application.csproj", "store-service/src/Store.Application/"]
COPY ["store-service/src/Store.Domain/Store.Domain.csproj", "store-service/src/Store.Domain/"]
COPY ["store-service/src/Store.Infrastructure/Store.Infrastructure.csproj", "store-service/src/Store.Infrastructure/"]

RUN dotnet restore "store-service/src/Store.API/Store.API.csproj"

COPY store-service/ store-service/

RUN dotnet publish "store-service/src/Store.API/Store.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Store.API.dll"]
