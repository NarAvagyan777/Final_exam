using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.RepositoryInterfaces; // 👈 միացրու սա UnitOfWork-ի համար
using Infrastructure.RepositoryInterfaces;

namespace Application.Services
{
    public class RecipesService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RecipesService(
            IRecipeRepository recipeRepo,
            IRatingRepository ratingRepo,
            IUnitOfWork unitOfWork)
        {
            _recipeRepo = recipeRepo;
            _ratingRepo = ratingRepo;
            _unitOfWork = unitOfWork;
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
                            UserId = r.UserId,
                            ImagePath = r.ImagePath
                        });
        }

        // ✅ Ստանալ մեկ recipe ըստ ID-ի (DTO)
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
                UserId = recipe.UserId,
                ImagePath = recipe.ImagePath
            };
        }

        // ✅ Ստանալ Recipe entity ըստ ID-ի (upload-ի կամ update-ի համար)
        public async Task<Recipe?> GetEntityByIdAsync(Guid id)
        {
            return await _recipeRepo.GetByIdAsync(id);
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
                UserId = dto.UserId
            };

            await _recipeRepo.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync(); // 👈 ապահով պահպանում

            return new RecipeDTO
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                Cuisine = entity.Cuisine,
                Difficulty = entity.Difficulty,
                AverageRating = entity.AverageRating,
                UserId = entity.UserId,
                ImagePath = entity.ImagePath
            };
        }

        // ✅ Ջնջել recipe
        public async Task<bool> DeleteAsync(Guid id)
        {
            var recipe = await _recipeRepo.GetByIdAsync(id);
            if (recipe == null) return false;

            await _recipeRepo.DeleteAsync(recipe);
            await _unitOfWork.SaveChangesAsync(); // 👈 ավելացրու այս պահպանումը
            return true;
        }

        // ✅ Գնահատել recipe (Transaction-ով)
        public async Task<bool> RateRecipeAsync(Guid recipeId, Guid userId, int score, string? comment)
        {
            var recipe = await _recipeRepo.GetByIdAsync(recipeId);
            if (recipe == null) return false;

            // 👇 Transaction-safe գործողություն
            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                // 1️⃣ Ստեղծում ենք նոր գնահատական
                var rating = new Rating
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    Score = score,
                    Comment = comment
                };

                await _ratingRepo.AddAsync(rating);

                // 2️⃣ Թարմացնում ենք միջին գնահատականը
                var allRatings = recipe.Ratings.Append(rating);
                recipe.AverageRating = allRatings.Average(r => r.Score);

                await _recipeRepo.UpdateAsync(recipe);
            });

            return true;
        }

        // ✅ Թարմացնել recipe (օր.՝ նկարի upload-ի ժամանակ)
        public async Task<RecipeDTO> UpdateAsync(Recipe recipe)
        {
            await _recipeRepo.UpdateAsync(recipe);
            await _unitOfWork.SaveChangesAsync(); // 👈 սա էլ ապահով պահպանում է փոփոխությունները

            return new RecipeDTO
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Cuisine = recipe.Cuisine,
                Difficulty = recipe.Difficulty,
                UserId = recipe.UserId,
                AverageRating = recipe.AverageRating,
                ImagePath = recipe.ImagePath
            };
        }
    }
}
