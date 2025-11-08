using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infrastructure.Data
{
    public class AppDbcontext : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<Ingredient> Ingredients => Set<Ingredient>();
        public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
        public DbSet<Nutrition> Nutritions => Set<Nutrition>();
        public DbSet<Rating> Ratings => Set<Rating>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Recipe - Ingredient (Many-to-Many)
            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => ri.Id);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId);

            // Recipe - Nutrition (1:1)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Nutrition)
                .WithOne(n => n.Recipe)
                .HasForeignKey<Nutrition>(n => n.RecipeId);

            // Recipe - User (N:1)
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId);

            // Rating - Recipe (N:1)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Recipe)
                .WithMany(re => re.Ratings)
                .HasForeignKey(r => r.RecipeId);

            // Rating - User (N:1)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
