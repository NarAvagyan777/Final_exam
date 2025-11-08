using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.RepositoryImplementations
{
    public class IngredientRepository : GenericRepository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(AppDbcontext context) : base(context) { }

        public async Task<Ingredient?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(i => i.Name.ToLower() == name.ToLower());
        }
    }
}
