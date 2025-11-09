# ============================
# 🧩 RecipeService Dockerfile
# Compatible with .NET 8.0
# ============================

# 1️⃣ Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2️⃣ SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ Copy project files first (for caching restore)
COPY ["RecipeService/RecipeService.csproj", "RecipeService/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# ✅ Restore dependencies
RUN dotnet restore "RecipeService/RecipeService.csproj"

# ✅ Copy all source code
COPY . .

# ✅ Build the app
WORKDIR "/src/RecipeService"
RUN dotnet build "RecipeService.csproj" -c Release -o /app/build

# ✅ Publish the app
FROM build AS publish
RUN dotnet publish "RecipeService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ✅ Runtime image (final stage)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecipeService.dll"]
