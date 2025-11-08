using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.RepositoryImplementations
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(AppDbcontext context) : base(context) { }

        public async Task<IEnumerable<Recipe>> GetRecipesWithRatingsAsync()
        {
            return await _dbSet
                .Include(r => r.Ratings)
                .Include(r => r.User)
                .Include(r => r.Nutrition)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToListAsync();
        }
    }
}
