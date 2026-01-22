# 1. Base stage - ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# 2. Build stage - .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files for caching
COPY src/CavoodleTrading.Api/CavoodleTrading.Api.csproj CavoodleTrading.Api/

# Standard restore
RUN dotnet restore CavoodleTrading.Api/CavoodleTrading.Api.csproj

# Copy and build
COPY src/ .
WORKDIR /src/CavoodleTrading.Api
RUN dotnet build CavoodleTrading.Api.csproj --no-restore -c Release -o /app/build

# 3. Publish stage
FROM build AS publish
RUN dotnet publish CavoodleTrading.Api.csproj -c Release -o /app/publish

# 4. Final stage
FROM base AS final
WORKDIR /app
ENV DOTNET_TieredPGO=1
ENV ASPNETCORE_URLS=http://+:5000
COPY --from=publish /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "CavoodleTrading.Api.dll"]
