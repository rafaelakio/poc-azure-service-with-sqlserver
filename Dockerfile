# Dockerfile for Link Manager - Multi-stage build

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["LinkManager.Web/LinkManager.Web.csproj", "LinkManager.Web/"]
RUN dotnet restore "LinkManager.Web/LinkManager.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/LinkManager.Web"
RUN dotnet build "LinkManager.Web.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "LinkManager.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 80

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
  CMD curl -f http://localhost/ || exit 1

# Run the application
ENTRYPOINT ["dotnet", "LinkManager.Web.dll"]
