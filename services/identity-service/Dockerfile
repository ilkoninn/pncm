FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Identity.API/Identity.API.csproj", "src/Identity.API/"]
COPY ["src/Identity.Application/Identity.Application.csproj", "src/Identity.Application/"]
COPY ["src/Identity.Domain/Identity.Domain.csproj", "src/Identity.Domain/"]
COPY ["src/Identity.Infrastructure/Identity.Infrastructure.csproj", "src/Identity.Infrastructure/"]

RUN dotnet restore "src/Identity.API/Identity.API.csproj"

COPY . .

RUN dotnet publish "src/Identity.API/Identity.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Identity.API.dll"]