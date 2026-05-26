FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Community.API/Community.API.csproj", "src/Community.API/"]
COPY ["src/Community.Application/Community.Application.csproj", "src/Community.Application/"]
COPY ["src/Community.Domain/Community.Domain.csproj", "src/Community.Domain/"]
COPY ["src/Community.Infrastructure/Community.Infrastructure.csproj", "src/Community.Infrastructure/"]

RUN dotnet restore "src/Community.API/Community.API.csproj"

COPY . .

RUN dotnet publish "src/Community.API/Community.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Community.API.dll"]
