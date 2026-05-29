FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY media-service/Protos/ media-service/Protos/

COPY ["identity-service/src/Identity.API/Identity.API.csproj", "identity-service/src/Identity.API/"]
COPY ["identity-service/src/Identity.Application/Identity.Application.csproj", "identity-service/src/Identity.Application/"]
COPY ["identity-service/src/Identity.Domain/Identity.Domain.csproj", "identity-service/src/Identity.Domain/"]
COPY ["identity-service/src/Identity.Infrastructure/Identity.Infrastructure.csproj", "identity-service/src/Identity.Infrastructure/"]

RUN dotnet restore "identity-service/src/Identity.API/Identity.API.csproj"

COPY identity-service/ identity-service/

RUN dotnet publish "identity-service/src/Identity.API/Identity.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
