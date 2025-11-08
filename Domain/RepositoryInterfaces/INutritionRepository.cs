using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.RepositoryInterfaces
{
    public interface INutritionRepository : IGenericRepository<Nutrition>
    {
        Task<Nutrition?> GetByRecipeIdAsync(Guid recipeId);
    }


}
