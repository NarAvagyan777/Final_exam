# ============================
# 🐇 Consumer Dockerfile
# Compatible with .NET 8.0 + MassTransit
# ============================

# 1️⃣ Runtime base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# 2️⃣ Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ Copy project files
COPY ["Consumer/Consumer.csproj", "Consumer/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# ✅ Restore dependencies
RUN dotnet restore "Consumer/Consumer.csproj"

# ✅ Copy source files
COPY . .

# ✅ Build project
WORKDIR "/src/Consumer"
RUN dotnet build "Consumer.csproj" -c Release -o /app/build

# ✅ Publish (no native host for smaller image)
FROM build AS publish
RUN dotnet publish "Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ✅ Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]
