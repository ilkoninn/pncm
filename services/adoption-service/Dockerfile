FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY pet-service/Protos/ pet-service/Protos/

COPY ["adoption-service/src/Adoption.API/Adoption.API.csproj", "adoption-service/src/Adoption.API/"]
COPY ["adoption-service/src/Adoption.Application/Adoption.Application.csproj", "adoption-service/src/Adoption.Application/"]
COPY ["adoption-service/src/Adoption.Domain/Adoption.Domain.csproj", "adoption-service/src/Adoption.Domain/"]
COPY ["adoption-service/src/Adoption.Infrastructure/Adoption.Infrastructure.csproj", "adoption-service/src/Adoption.Infrastructure/"]

RUN dotnet restore "adoption-service/src/Adoption.API/Adoption.API.csproj"

COPY adoption-service/ adoption-service/

RUN dotnet publish "adoption-service/src/Adoption.API/Adoption.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Adoption.API.dll"]
