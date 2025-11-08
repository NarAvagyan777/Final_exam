using Domain.Entities;
using Infrastructure.Data;
using System;

namespace RecipeService.Infrastructure
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbcontext context)
        {
            if (!context.Ingredients.Any())
            {
                context.Ingredients.AddRange(
                    new Ingredient { Name = "Salt" },
                    new Ingredient { Name = "Sugar" },
                    new Ingredient { Name = "Flour" },
                    new Ingredient { Name = "Egg" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}
