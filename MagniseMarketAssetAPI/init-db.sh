#!/bin/bash
set -e

echo "Applying EF Core migrations..."
dotnet ef database update --project /src/MagniseMarketAssetAPI/MagniseMarketAssetAPI.csproj
echo "Migrations applied."

echo "Starting the application..."
dotnet MagniseMarketAssetAPI.dll
