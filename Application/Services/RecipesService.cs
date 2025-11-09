using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain.RepositoryInterfaces;
using Infrastructure.Messaging; // 👈 RabbitMQ Publisher-ի import
using Infrastructure.RepositoryInterfaces;

namespace Application.Services
{
    public class RecipesService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IRatingRepository _ratingRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RabbitMqPublisher _publisher; // 👈 RabbitMQ publisher

        public RecipesService(
            IRecipeRepository recipeRepo,
            IRatingRepository ratingRepo,
            IUnitOfWork unitOfWork,
            RabbitMqPublisher publisher) // 👈 publisher dependency
        {
            _recipeRepo = recipeRepo;
            _ratingRepo = ratingRepo;
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        // ✅ Էջավորում + Ֆիլտրում
        public async Task<PagedResult<RecipeDTO>> GetAllAsync(
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

            var totalCount = query.Count();

            var paged = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<RecipeDTO>
            {
                Items = paged.Select(r => new RecipeDTO
                {
                    Id = r.Id,
                    Title = r.Title,
                    Description = r.Description,
                    Cuisine = r.Cuisine,
                    Difficulty = r.Difficulty,
                    AverageRating = r.AverageRating,
                    UserId = r.UserId,
                    ImagePath = r.ImagePath
                }),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // ✅ Ստանալ մեկ recipe DTO
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

        // ✅ Ստանալ recipe entity
        public async Task<Recipe?> GetEntityByIdAsync(Guid id)
        {
            return await _recipeRepo.GetByIdAsync(id);
        }

        // ✅ Ստեղծել recipe և ուղարկել հաղորդագրություն RabbitMQ-ին
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
            await _unitOfWork.SaveChangesAsync();

            // 📨 Ուղարկում ենք հաղորդագրություն RabbitMQ-ին
            _publisher.Publish(new
            {
                Event = "RecipeCreated",
                RecipeId = entity.Id,
                Title = entity.Title,
                Cuisine = entity.Cuisine,
                Difficulty = entity.Difficulty,
                UserId = entity.UserId
            });

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
            await _unitOfWork.SaveChangesAsync();

            // 📨 RabbitMQ event (optional)
            _publisher.Publish(new
            {
                Event = "RecipeDeleted",
                RecipeId = id
            });

            return true;
        }

        // ✅ Գնահատել recipe (Transaction-ով)
        public async Task<bool> RateRecipeAsync(Guid recipeId, Guid userId, int score, string? comment)
        {
            var recipe = await _recipeRepo.GetByIdAsync(recipeId);
            if (recipe == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                var rating = new Rating
                {
                    RecipeId = recipeId,
                    UserId = userId,
                    Score = score,
                    Comment = comment
                };

                await _ratingRepo.AddAsync(rating);

                var allRatings = recipe.Ratings.Append(rating);
                recipe.AverageRating = allRatings.Average(r => r.Score);

                await _recipeRepo.UpdateAsync(recipe);
            });

            // 📨 Event — Rating added
            _publisher.Publish(new
            {
                Event = "RecipeRated",
                RecipeId = recipeId,
                UserId = userId,
                Score = score,
                Comment = comment
            });

            return true;
        }

        // ✅ Թարմացնել recipe (օր.՝ upload-ի ժամանակ)
        public async Task<RecipeDTO> UpdateAsync(Recipe recipe)
        {
            await _recipeRepo.UpdateAsync(recipe);
            await _unitOfWork.SaveChangesAsync();

            // 📨 Event — Recipe updated
            _publisher.Publish(new
            {
                Event = "RecipeUpdated",
                RecipeId = recipe.Id,
                Title = recipe.Title,
                UpdatedAt = DateTime.UtcNow
            });

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
