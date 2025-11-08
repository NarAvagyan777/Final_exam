using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.RepositoryInterfaces;

namespace Application.Services
{
    public class RecipesService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRatingRepository _ratingRepo;

        public RecipesService(IRecipeRepository recipeRepo, IRatingRepository ratingRepo)
        {
            _recipeRepo = recipeRepo;
            _ratingRepo = ratingRepo;
        }

        // ✅ Բոլոր recipe-ների ստացում էջայնությամբ և ֆիլտրերով
        public async Task<IEnumerable<RecipeDTO>> GetAllAsync(
            int page = 1,
            int pageSize = 10,
            string? cuisine = null,
            string? difficulty = null)
        {
            var recipes = await _recipeRepo.GetAllAsync();

            var query = recipes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(cuisine))
                query = query.Where(r => r.Cuisine.ToLower() == cuisine.ToLower());

            if (!string.IsNullOrWhiteSpace(difficulty))
                query = query.Where(r => r.Difficulty.ToLower() == difficulty.ToLower());

            return query.Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .Select(r => new RecipeDTO
                        {
                            Id = r.Id,
                            Title = r.Title,
                            Description = r.Description,
                            Cuisine = r.Cuisine,
                            Difficulty = r.Difficulty,
                            AverageRating = r.AverageRating,
                            UserId = r.UserId
                        });
        }

        // ✅ Ստանալ մեկ recipe ըստ ID-ի
        public async Task<RecipeDTO?> GetByIdAsync(Guid id)
        {
            var recipe = await _recipeRepo.GetByIdAsync(id);
            if (recipe == null) return null;

            return new RecipeDTO
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Cuisine = recipe.Cuisine,
                Difficulty = recipe.Difficulty,
                AverageRating = recipe.AverageRating,
                UserId = recipe.UserId
            };
        }

        // ✅ Ստեղծել նոր recipe (օգտագործում է CreateRecipeDto)
        public async Task<RecipeDTO> CreateAsync(CreateRecipeDto dto)
        {
            var entity = new Recipe
            {
                Title = dto.Title,
                Description = dto.Description,
                Cuisine = dto.Cuisine,
                Difficulty = dto.Difficulty,
              //  AverageRating = dto.AverageRating,
                UserId = dto.UserId
                // Id-ը ավտոմատ կստեղծվի Recipe entity-ում Guid.NewGuid()-ով
            };

            await _recipeRepo.AddAsync(entity);

            // վերադարձնում ենք RecipeDTO (Id արդեն ստեղծված է)
            return new RecipeDTO
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Cuisine = entity.Cuisine,
                Difficulty = entity.Difficulty,
                AverageRating = entity.AverageRating,
                UserId = entity.UserId
            };
        }

        // ✅ Ջնջել recipe
        public async Task<bool> DeleteAsync(Guid id)
        {
            var recipe = await _recipeRepo.GetByIdAsync(id);
            if (recipe == null) return false;

            await _recipeRepo.DeleteAsync(recipe);
            return true;
        }

        // ✅ Գնահատել recipe (RateRecipeAsync)
        public async Task<bool> RateRecipeAsync(Guid recipeId, Guid userId, int score, string? comment)
        {
            var recipe = await _recipeRepo.GetByIdAsync(recipeId);
            if (recipe == null) return false;

            var rating = new Rating
            {
                RecipeId = recipeId,
                UserId = userId,
                Score = score,
                Comment = comment
            };

            await _ratingRepo.AddAsync(rating);

            // Պարզ օրինակ՝ թարմացնում ենք միջին գնահատականը (եթե անհրաժեշտ է)
           // recipe.AverageRating = (recipe.AverageRating + score) / 2;
           // await _recipeRepo.UpdateAsync(recipe);

            return true;
        }
    }
}
