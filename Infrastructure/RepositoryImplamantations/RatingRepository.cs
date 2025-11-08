using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.RepositoryImplementations
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        public RatingRepository(AppDbcontext context) : base(context) { }

        public async Task<IEnumerable<Rating>> GetByRecipeIdAsync(Guid recipeId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.RecipeId == recipeId)
                .ToListAsync();
        }
    }
}
