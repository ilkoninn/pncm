FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Media.API/Media.API.csproj", "src/Media.API/"]
COPY ["src/Media.Application/Media.Application.csproj", "src/Media.Application/"]
COPY ["src/Media.Domain/Media.Domain.csproj", "src/Media.Domain/"]
COPY ["src/Media.Infrastructure/Media.Infrastructure.csproj", "src/Media.Infrastructure/"]

RUN dotnet restore "src/Media.API/Media.API.csproj"

COPY . .

RUN dotnet publish "src/Media.API/Media.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Media.API.dll"]