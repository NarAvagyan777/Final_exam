using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.RepositoryInterfaces;

namespace Application.Services
{
    public class NutritionService : INutritionService
    {
        private readonly INutritionRepository _nutritionRepo;

        public NutritionService(INutritionRepository nutritionRepo)
        {
            _nutritionRepo = nutritionRepo;
        }

        // 🔹 Վերադարձնում է տվյալը ըստ RecipeId-ի
        public async Task<NutritionDto?> GetByRecipeIdAsync(Guid recipeId)
        {
            var entity = await _nutritionRepo.GetByRecipeIdAsync(recipeId);
            if (entity == null)
                return null;

            return new NutritionDto
            {
                Id = entity.Id,
                RecipeId = entity.RecipeId,
                Calories = entity.Calories,
                Protein = entity.Protein,
                Carbs = entity.Carbs,
                Fat = entity.Fat,
               // CreatedAt = entity.CreatedAt,
               // UpdatedAt = entity.UpdatedAt
            };
        }

        // 🔹 Ստեղծում է նոր Nutrition (օգտագործում է CreateNutritionDto)
        public async Task<NutritionDto> CreateAsync(CreateNutritionDto dto)
        {
            var entity = new Nutrition
            {
                Id = Guid.NewGuid(),
                RecipeId = dto.RecipeId,
                Calories = dto.Calories,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat,
                CreatedAt = DateTime.UtcNow
            };

            await _nutritionRepo.AddAsync(entity);

            return new NutritionDto
            {
                Id = entity.Id,
                RecipeId = entity.RecipeId,
                Calories = entity.Calories,
                Protein = entity.Protein,
                Carbs = entity.Carbs,
                Fat = entity.Fat,
              //  CreatedAt = entity.CreatedAt
            };
        }

        // 🔹 Թարմացնում է գոյություն ունեցող Nutrition
        public async Task<bool> UpdateAsync(NutritionDto dto)
        {
            var entity = await _nutritionRepo.GetByRecipeIdAsync(dto.RecipeId);
            if (entity == null)
                return false;

            entity.Calories = dto.Calories;
            entity.Protein = dto.Protein;
            entity.Carbs = dto.Carbs;
            entity.Fat = dto.Fat;
            entity.UpdatedAt = DateTime.UtcNow;

            await _nutritionRepo.UpdateAsync(entity);
            return true;
        }
    }
}
