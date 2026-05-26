FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Notification.API/Notification.API.csproj", "src/Notification.API/"]
COPY ["src/Notification.Application/Notification.Application.csproj", "src/Notification.Application/"]
COPY ["src/Notification.Domain/Notification.Domain.csproj", "src/Notification.Domain/"]
COPY ["src/Notification.Infrastructure/Notification.Infrastructure.csproj", "src/Notification.Infrastructure/"]
RUN dotnet restore "src/Notification.API/Notification.API.csproj"
COPY . .
RUN dotnet publish "src/Notification.API/Notification.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Notification.API.dll"]
