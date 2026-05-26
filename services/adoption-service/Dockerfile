FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Adoption.API/Adoption.API.csproj", "src/Adoption.API/"]
COPY ["src/Adoption.Application/Adoption.Application.csproj", "src/Adoption.Application/"]
COPY ["src/Adoption.Domain/Adoption.Domain.csproj", "src/Adoption.Domain/"]
COPY ["src/Adoption.Infrastructure/Adoption.Infrastructure.csproj", "src/Adoption.Infrastructure/"]

RUN dotnet restore "src/Adoption.API/Adoption.API.csproj"

COPY . .

RUN dotnet publish "src/Adoption.API/Adoption.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Adoption.API.dll"]
