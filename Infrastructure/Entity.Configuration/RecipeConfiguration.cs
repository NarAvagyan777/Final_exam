using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeService.Infrastructure.Entity.Configuration
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.Property(r => r.Title).IsRequired().HasMaxLength(100);
            builder.Property(r => r.Cuisine).HasMaxLength(50);
            builder.Property(r => r.Difficulty).HasMaxLength(50);
        }
    }
}
