FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Pet.API/Pet.API.csproj", "src/Pet.API/"]
COPY ["src/Pet.Application/Pet.Application.csproj", "src/Pet.Application/"]
COPY ["src/Pet.Domain/Pet.Domain.csproj", "src/Pet.Domain/"]
COPY ["src/Pet.Infrastructure/Pet.Infrastructure.csproj", "src/Pet.Infrastructure/"]

RUN dotnet restore "src/Pet.API/Pet.API.csproj"

COPY . .

RUN dotnet publish "src/Pet.API/Pet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Pet.API.dll"]