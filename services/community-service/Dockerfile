FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY media-service/Protos/ media-service/Protos/

COPY ["community-service/src/Community.API/Community.API.csproj", "community-service/src/Community.API/"]
COPY ["community-service/src/Community.Application/Community.Application.csproj", "community-service/src/Community.Application/"]
COPY ["community-service/src/Community.Domain/Community.Domain.csproj", "community-service/src/Community.Domain/"]
COPY ["community-service/src/Community.Infrastructure/Community.Infrastructure.csproj", "community-service/src/Community.Infrastructure/"]

RUN dotnet restore "community-service/src/Community.API/Community.API.csproj"

COPY community-service/ community-service/

RUN dotnet publish "community-service/src/Community.API/Community.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Community.API.dll"]
