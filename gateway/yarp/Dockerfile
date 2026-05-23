FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Gateway.API/Gateway.API.csproj", "src/Gateway.API/"]

RUN dotnet restore "src/Gateway.API/Gateway.API.csproj"

COPY . .

RUN dotnet publish "src/Gateway.API/Gateway.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.API.dll"]