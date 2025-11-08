using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.DTOs;

namespace Domain.Interfaces
{
    public interface INutritionService
    {
        Task<NutritionDto?> GetByRecipeIdAsync(Guid recipeId);
        Task<NutritionDto> CreateAsync(CreateNutritionDto dto);
        Task<bool> UpdateAsync(NutritionDto dto);
    }
}
