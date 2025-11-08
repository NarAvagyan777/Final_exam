using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.RepositoryImplementations
{
    public class NutritionRepository : GenericRepository<Nutrition>, INutritionRepository
    {
        public NutritionRepository(AppDbcontext context) : base(context) { }

        public async Task<Nutrition?> GetByRecipeIdAsync(Guid recipeId)
        {
            return await _dbSet.FirstOrDefaultAsync(n => n.RecipeId == recipeId);
        }
    }
}
