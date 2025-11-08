using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.RepositoryInterfaces;

namespace Application.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepo;

        public IngredientService(IIngredientRepository ingredientRepo)
        {
            _ingredientRepo = ingredientRepo;
        }

        // ✅ Վերադարձնում է բոլոր ingredient-ները (Id + Name)
        public async Task<IEnumerable<IngridientDto>> GetAllAsync()
        {
            var ingredients = await _ingredientRepo.GetAllAsync();

            return ingredients.Select(i => new IngridientDto
            {
                Id = i.Id,
                Name = i.Name
            });
        }

        // ✅ Ստեղծում է նոր ingredient (ընդունում է միայն Name)
        public async Task<IngridientDto> CreateAsync(CreateIngredientDto dto)
        {
            // Սերվերի կողմում ավտոմատ գեներացվում է Guid
            var entity = new Ingredient
            {
                Name = dto.Name
            };

            await _ingredientRepo.AddAsync(entity);

            // EF Core-ը արդեն գրանցել է գեներացված Id-ն
            return new IngridientDto
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        // ✅ Ջնջում է ըստ Guid Id
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _ingredientRepo.GetByIdAsync(id);
            if (entity == null)
                return false;

            await _ingredientRepo.DeleteAsync(entity);
            return true;
        }
    }
}
