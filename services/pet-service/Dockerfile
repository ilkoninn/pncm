FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY media-service/Protos/ media-service/Protos/

COPY ["pet-service/src/Pet.API/Pet.API.csproj", "pet-service/src/Pet.API/"]
COPY ["pet-service/src/Pet.Application/Pet.Application.csproj", "pet-service/src/Pet.Application/"]
COPY ["pet-service/src/Pet.Domain/Pet.Domain.csproj", "pet-service/src/Pet.Domain/"]
COPY ["pet-service/src/Pet.Infrastructure/Pet.Infrastructure.csproj", "pet-service/src/Pet.Infrastructure/"]

RUN dotnet restore "pet-service/src/Pet.API/Pet.API.csproj"

COPY pet-service/ pet-service/

RUN dotnet publish "pet-service/src/Pet.API/Pet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pet.API.dll"]
