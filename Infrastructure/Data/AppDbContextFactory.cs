using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbcontext>
    {
        public AppDbcontext CreateDbContext(string[] args)
        {
            // ✅ Ճիշտ ուղի դեպի RecipeService-ի appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../RecipeService");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("❌ Connection string not found in appsettings.json");

            Console.WriteLine($"🔗 Connection: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbcontext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbcontext(optionsBuilder.Options);
        }
    }
}
