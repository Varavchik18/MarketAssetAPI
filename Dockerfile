# Використовуйте відповідний базовий образ для .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MagniseMarketAssetAPI/MagniseMarketAssetAPI.csproj", "MagniseMarketAssetAPI/"]
RUN dotnet restore "MagniseMarketAssetAPI/MagniseMarketAssetAPI.csproj"
COPY . .
WORKDIR "/src/MagniseMarketAssetAPI"
RUN dotnet build "MagniseMarketAssetAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MagniseMarketAssetAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MagniseMarketAssetAPI.dll"]
